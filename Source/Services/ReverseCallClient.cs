// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Tenancy;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IReverseCallClient{TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client using.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client using.</typeparam>
    public class ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
        : IDisposable, IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse>
        where TClientMessage : IMessage
        where TServerMessage : IMessage
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _protocol;
        readonly TimeSpan _pingInterval;
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;
        readonly SemaphoreSlim _writeResponseSemaphore = new SemaphoreSlim(1);
        readonly object _connectLock = new object();
        readonly object _handleLock = new object();
        IClientStreamWriter<TClientMessage> _clientToServer;
        IAsyncStreamReader<TServerMessage> _serverToClient;
        bool _connecting;
        bool _connectionEstablished;
        bool _startedHandling;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>/// <param name="protocol">The the reverse call protocol that will be used to connect to the server.</param>
        /// <param name="pingInterval">The interval at which to request pings from the server to keep the reverse call alive.</param>
        /// <param name="caller">The caller that will be used to perform the method call.</param>
        /// <param name="executionContext">The execution context to use while initiating the reverse call.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ReverseCallClient(
            IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol,
            TimeSpan pingInterval,
            IPerformMethodCalls caller,
            ExecutionContext executionContext,
            ILogger logger)
        {
            ThrowIfInvalidPingInterval(pingInterval);
            _protocol = protocol;
            _pingInterval = pingInterval;
            _caller = caller;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public TConnectResponse ConnectResponse { get; private set; }

        /// <inheritdoc/>
        public async Task<bool> Connect(TConnectArguments connectArguments, CancellationToken cancellationToken)
        {
            ThrowIfConnecting();
            lock (_connectLock)
            {
                ThrowIfConnecting();
                _connecting = true;
            }

            var streamingCall = _caller.Call(_protocol, cancellationToken);
            _clientToServer = streamingCall.RequestStream;
            _serverToClient = streamingCall.ResponseStream;
            var callContext = CreateReverseCallArgumentsContext();
            _protocol.SetConnectArgumentsContextIn(callContext, connectArguments);
            var message = _protocol.CreateMessageFrom(connectArguments);

            await _clientToServer.WriteAsync(message).ConfigureAwait(false);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(_pingInterval.Multiply(3));

            try
            {
                if (await _serverToClient.MoveNext(linkedCts.Token).ConfigureAwait(false))
                {
                    var response = _protocol.GetConnectResponseFrom(_serverToClient.Current);
                    if (response != null)
                    {
                        _logger.LogTrace("Received connect response");
                        ConnectResponse = response;
                        _connectionEstablished = true;
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Did not receive connect response. Server message did not contain the connect response");
                    }
                }
                else
                {
                    _logger.LogWarning("Did not receive connect response. Server stream was empty");
                }

                await _clientToServer.CompleteAsync().ConfigureAwait(false);
                return false;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Reverse Call Client was cancelled by client while connecting");
                }
                else
                {
                    _logger.LogWarning("Reverse Call Client was cancelled by server while connecting");
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public async Task Handle(IReverseCallHandler<TRequest, TResponse> handler, CancellationToken cancellationToken)
        {
            ThrowIfConnectionNotEstablished();
            ThrowIfAlreadyStartedHandling();
            lock (_handleLock)
            {
                ThrowIfAlreadyStartedHandling();
                _startedHandling = true;
            }

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(_pingInterval.Multiply(3));
            try
            {
                while (await _serverToClient.MoveNext(linkedCts.Token).ConfigureAwait(false))
                {
                    var message = _serverToClient.Current;
                    var ping = _protocol.GetPingFrom(message);
                    var request = _protocol.GetRequestFrom(message);
                    if (ping != null)
                    {
                        _logger.LogTrace("Received ping");
                        await WritePong(cancellationToken).ConfigureAwait(false);
                    }
                    else if (request != null)
                    {
                        _ = Task.Run(() => OnReceivedRequest(handler, request, cancellationToken));
                    }
                    else
                    {
                        _logger.LogWarning("Received message from Reverse Call Dispatcher, but it was not a request or a ping");
                    }

                    linkedCts.CancelAfter(_pingInterval.Multiply(3));
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Reverse Call Client was cancelled by client while handling requests");
                    return;
                }

                if (!linkedCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Reverse Call Client was cancelled by server while handling requests");
                    return;
                }

                throw new PingTimedOut(_pingInterval);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writeResponseSemaphore.Dispose();
                }

                _disposed = true;
            }
        }

        ReverseCallArgumentsContext CreateReverseCallArgumentsContext()
            => new ReverseCallArgumentsContext
            {
                HeadId = Guid.NewGuid().ToProtobuf(),
                ExecutionContext = _executionContext.ToProtobuf(),
                PingInterval = Duration.FromTimeSpan(_pingInterval),
            };

        async Task WritePong(CancellationToken cancellationToken)
        {
            await _writeResponseSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Reverse Call Client was cancelled before it could respond with pong");
                    return;
                }

                var message = _protocol.CreateMessageFrom(new Pong());

                _logger.LogTrace("Writing pong");
                await _clientToServer.WriteAsync(message).ConfigureAwait(false);
            }
            finally
            {
                _writeResponseSemaphore.Release();
            }
        }

        async Task OnReceivedRequest(IReverseCallHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var requestContext = _protocol.GetRequestContextFrom(request);
                var callId = requestContext.CallId.ToGuid();

                TResponse response;
                try
                {
                    _logger.LogTrace("Handling request with call '{CallId}'", callId);

                    var executionContext = _executionContext
                        .ForTenant(requestContext.ExecutionContext.TenantId.To<TenantId>())
                        .ForCorrelation(requestContext.ExecutionContext.CorrelationId.To<CorrelationId>());

                    response = await handler.Handle(request, executionContext, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while invoking handler for call '{CallId}'", callId);
                    return;
                }

                await _writeResponseSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await WriteResponse(response, callId, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while writing response for call '{CallId}'", callId);
                }
                finally
                {
                    _writeResponseSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while handling received request");
            }
        }

        Task WriteResponse(TResponse response, Guid callId, CancellationToken cancellationToken)
        {
            var responseContext = new ReverseCallResponseContext { CallId = callId.ToProtobuf() };
            _protocol.SetResponseContextIn(responseContext, response);
            var message = _protocol.CreateMessageFrom(response);
            if (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("Writing response for call '{CallId}'", callId);
                return _clientToServer.WriteAsync(message);
            }

            _logger.LogTrace("Client was cancelled while writing response for call '{CallId}'", callId);

            return Task.CompletedTask;
        }

        void ThrowIfInvalidPingInterval(TimeSpan pingInterval)
        {
            if (pingInterval.TotalMilliseconds <= 0) throw new PingIntervalNotGreaterThanZero();
        }

        void ThrowIfConnecting()
        {
            if (_connecting)
            {
                throw new ReverseCallClientAlreadyCalledConnect();
            }
        }

        void ThrowIfAlreadyStartedHandling()
        {
            if (_startedHandling)
            {
                throw new ReverseCallClientAlreadyStartedHandling();
            }
        }

        void ThrowIfConnectionNotEstablished()
        {
            if (!_connectionEstablished)
            {
                throw new ReverseCallClientNotConnected();
            }
        }
    }
}
