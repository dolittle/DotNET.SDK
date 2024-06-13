// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Tenancy;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services;

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
    : IDisposable, IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse, TClientMessage>
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
    readonly ITenantScopedProviders _tenantScopedProviders;
    readonly ILogger _logger;
    readonly SemaphoreSlim _writeResponseSemaphore = new(1);
    readonly object _connectLock = new();
    readonly object _handleLock = new();
    IClientStreamWriter<TClientMessage>? _clientToServer;
    IAsyncStreamReader<TServerMessage>? _serverToClient;
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
    /// <param name="tenantScopedProviders">The <see cref="ITenantScopedProviders"/> for resolving a <see cref="IServiceProvider"/> for a specific <see cref="TenantId"/>.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public ReverseCallClient(
        IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol,
        TimeSpan pingInterval,
        IPerformMethodCalls caller,
        ExecutionContext executionContext,
        ITenantScopedProviders tenantScopedProviders,
        ILogger logger)
    {
        ThrowIfInvalidPingInterval(pingInterval);
        _protocol = protocol;
        _pingInterval = pingInterval;
        _caller = caller;
        _executionContext = executionContext;
        _tenantScopedProviders = tenantScopedProviders;
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
        var connectMessage = _protocol.CreateMessageFrom(connectArguments);

        // ReSharper disable once MethodSupportsCancellation
        await _clientToServer.WriteAsync(connectMessage).ConfigureAwait(false);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (ShouldPingPong)
        {
            linkedCts.CancelAfter(_pingInterval.Multiply(3));
        }

        try
        {
            while (await _serverToClient.MoveNext(linkedCts.Token).ConfigureAwait(false))
            {
                var message = _serverToClient.Current;
                var ping = _protocol.GetPingFrom(message);
                var response = _protocol.GetConnectResponseFrom(message);
                if (ping != null)
                {
                    _logger.ReceivedPing();
                    await WritePong(cancellationToken).ConfigureAwait(false);
                }
                else if (response != null)
                {
                    _logger.ReceivedConnectResponse();
                    ConnectResponse = response;
                    _connectionEstablished = true;
                    return true;
                }
                else
                {
                    _logger.ReceivedNonPingOrResponseDuringConnect();
                }

                if (ShouldPingPong)
                {
                    linkedCts.CancelAfter(_pingInterval.Multiply(3));
                }
            }

            _logger.TimedOutDuringConnect();
            await _clientToServer.CompleteAsync().ConfigureAwait(false);
            return false;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.CancelledByClientDuringConnect();
            }
            else
            {
                _logger.CancelledByServerDuringConnect();
            }

            return false;
        }
    }

    public Task WriteMessage(TClientMessage message, CancellationToken token)
    {
        // ReSharper disable once MethodSupportsCancellation
        return _clientToServer!.WriteAsync(message);
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
        if (ShouldPingPong)
        {
            linkedCts.CancelAfter(_pingInterval.Multiply(3));
        }
        try
        {
            while (await _serverToClient.MoveNext(linkedCts.Token).ConfigureAwait(false))
            {
                var message = _serverToClient.Current;
                var ping = _protocol.GetPingFrom(message);
                var request = _protocol.GetRequestFrom(message);
                if (ping != null)
                {
                    _logger.ReceivedPing();
                    await WritePong(cancellationToken).ConfigureAwait(false);
                }
                else if (request != null)
                {
                    _ = Task.Run(() => OnReceivedRequest(handler, request, cancellationToken), CancellationToken.None);
                }
                else if (_protocol.IsDisconnectAck(message))
                {
                    // The server has acknowledged our disconnect request, and completed in-flight requests.
                    // We can now safely complete the stream.
                    _logger.ReceivedDisconnectAck();
                    await _clientToServer!.CompleteAsync().ConfigureAwait(false);
                    return;
                }
                else
                {
                    _logger.ReceivedNonPingOrRequestDuringHandling();
                }

                if (ShouldPingPong)
                {
                    linkedCts.CancelAfter(_pingInterval.Multiply(3));
                }
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.CancelledByClientDuringHandling();
                return;
            }

            if (linkedCts.IsCancellationRequested || cancellationToken.IsCancellationRequested)
            {
                throw new PingTimedOut(_pingInterval);
            }

            _logger.CancelledByServerDuringHandling();
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
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _writeResponseSemaphore.Dispose();
        }

        _disposed = true;
    }

    ReverseCallArgumentsContext CreateReverseCallArgumentsContext()
        => new()
        {
            HeadId = Guid.NewGuid().ToProtobuf(),
            ExecutionContext = _executionContext.ToProtobuf(),
            PingInterval = ShouldPingPong ? Duration.FromTimeSpan(_pingInterval) : Duration.FromTimeSpan(TimeSpan.FromSeconds(Duration.MaxSeconds))
        };

    async Task WritePong(CancellationToken cancellationToken)
    {
        await _writeResponseSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.CancelledBeforePongCouldBeSent();
                return;
            }

            var message = _protocol.CreateMessageFrom(new Pong());

            _logger.WritingPong();
            // ReSharper disable once MethodSupportsCancellation
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
                _logger.HandlingRequest(callId);

                var executionContext = _executionContext
                    .ForTenant(requestContext.ExecutionContext.TenantId.To<TenantId>())
                    .ForCorrelation(requestContext.ExecutionContext.CorrelationId.To<CorrelationId>());

                response = await handler.Handle(request, executionContext, _tenantScopedProviders.ForTenant(executionContext.Tenant), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.ErrorWhileInvokingHandlerFor(callId, ex);
                return;
            }

            await _writeResponseSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await WriteResponse(response, callId, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.ErrorWhileWritingResponseFor(callId, ex);
            }
            finally
            {
                _writeResponseSemaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileHandlingRequest(ex);
        }
    }

    Task WriteResponse(TResponse response, Guid callId, CancellationToken cancellationToken)
    {
        var responseContext = new ReverseCallResponseContext { CallId = callId.ToProtobuf() };
        _protocol.SetResponseContextIn(responseContext, response);
        var message = _protocol.CreateMessageFrom(response);
        if (!cancellationToken.IsCancellationRequested)
        {
            _logger.WritingResponseFor(callId);
            return _clientToServer.WriteAsync(message);
        }

        _logger.CancelledWhileWritingResponseFor(callId);

        return Task.CompletedTask;
    }

    bool ShouldPingPong => _pingInterval != TimeSpan.MaxValue;

    static void ThrowIfInvalidPingInterval(TimeSpan pingInterval)
    {
        if (pingInterval.TotalMilliseconds <= 0)
        {
            throw new PingIntervalNotGreaterThanZero();
        }
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
