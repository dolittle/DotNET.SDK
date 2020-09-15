// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// An implementation of <see cref="IReverseCallClient{TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
    public class ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
        : IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse>
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
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;
        readonly IObservable<TConnectResponse> _observable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to send to the server to start the reverse call protocol.</param>
        /// <param name="handler">The handler that will handle requests from the server.</param>
        /// <param name="protocol">The the reverse call protocol that will be used to connect to the server.</param>
        /// <param name="pingInterval">The interval at which to request pings from the server to keep the reverse call alive.</param>
        /// <param name="caller">The caller that will be used to perform the method call.</param>
        /// <param name="executionContextManager">The execution context manager that will be used to set the execution context while handling requests from the server.</param>
        /// <param name="logger">The logger that will be used to log messages while performing the reverse call.</param>
        public ReverseCallClient(
            TConnectArguments arguments,
            IReverseCallHandler<TRequest, TResponse> handler,
            IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol,
            TimeSpan pingInterval,
            IPerformMethodCalls caller,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            Arguments = arguments;
            Handler = handler;
            _protocol = protocol;
            _pingInterval = pingInterval;
            _caller = caller;
            _executionContextManager = executionContextManager;
            _logger = logger;
            _observable = CreateObservable();
        }

        /// <inheritdoc/>
        public TConnectArguments Arguments { get; }

        /// <inheritdoc/>
        public IReverseCallHandler<TRequest, TResponse> Handler { get; }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TConnectResponse> observer)
            => _observable.Subscribe(observer);

        IObservable<TConnectResponse> CreateObservable()
            => Observable.Create<TConnectResponse>((observer) =>
                {
                    var toClientMessages = new Subject<TServerMessage>();

                    var validMessages = toClientMessages.Skip(1).Where(MessageIsPingOrRequest).Timeout(_pingInterval * 3);
                    var pings = validMessages.Where(MessageIsPing);
                    var requests = validMessages.Where(MessageIsRequest);

                    var pongs = pings
                        .Select(_protocol.GetPingFrom)
                        .Select(_ => new Pong())
                        .Select(_protocol.CreateMessageFrom);

                    var responses = requests
                        .Select(_protocol.GetRequestFrom)
                        .Where(RequestIsValid)
                        .Select(request => Observable.FromAsync((token) => HandleRequest(request, token)))
                        .Merge()
                        .Select(_protocol.CreateMessageFrom);

                    var connectArguments = Arguments;
                    var connectContext = CreateReverseCallArgumentsContext();
                    _protocol.SetConnectArgumentsContextIn(connectContext, connectArguments);
                    var connectMessage = _protocol.CreateMessageFrom(connectArguments);

                    var toServerMessages = pongs.Merge(responses).StartWith(connectMessage);

                    var connectResponse = toClientMessages
                        .Take(1)
                        .Select(_ =>
                            {
                                var response = _protocol.GetConnectResponseFrom(_);
                                if (response == null)
                                {
                                    return Notification.CreateOnError<TConnectResponse>(new DidNotReceiveConnectResponse());
                                }

                                return Notification.CreateOnNext(response);
                            })
                        .DefaultIfEmpty(Notification.CreateOnError<TConnectResponse>(new DidNotReceiveConnectResponse()))
                        .Dematerialize();

                    var errorsAndCompletion = toClientMessages
                        .Where(_ => false)
                        .Select(_protocol.GetConnectResponseFrom)
                        .Catch((TimeoutException _) => Observable.Throw<TConnectResponse>(new PingTimedOut(_pingInterval)));

                    connectResponse.Merge(errorsAndCompletion).Subscribe(observer);
                    return _caller.Call(_protocol, toServerMessages).Subscribe(toClientMessages);
                });

        ReverseCallArgumentsContext CreateReverseCallArgumentsContext()
            => new ReverseCallArgumentsContext
                {
                    HeadId = Guid.NewGuid().ToProtobuf(),
                    ExecutionContext = _executionContextManager.Current.ToProtobuf(),
                    PingInterval = Duration.FromTimeSpan(_pingInterval),
                };

        bool MessageIsPing(TServerMessage message)
            => _protocol.GetPingFrom(message) != null;

        bool MessageIsRequest(TServerMessage message)
            => _protocol.GetRequestFrom(message) != null;

        bool MessageIsPingOrRequest(TServerMessage message)
        {
            if (MessageIsPing(message) || MessageIsRequest(message))
            {
                return true;
            }

            _logger.LogWarning("Received message from Reverse Call Dispatcher, but it was not a request or a ping");
            return false;
        }

        bool RequestIsValid(TRequest request)
        {
            var context = _protocol.GetRequestContextFrom(request);
            if (context == null)
            {
                _logger.LogWarning("Received request from Reverse Call Dispatcher, but it did not contain a Reverse Call Context");
                return false;
            }
            else if (context.ExecutionContext == null)
            {
                _logger.LogWarning("Received request from Reverse Call Dispatcher, but it did not contain an Execution Context");
                return false;
            }

            return true;
        }

        async Task<TResponse> HandleRequest(TRequest request, CancellationToken token)
        {
            var requestContext = _protocol.GetRequestContextFrom(request);
            var executionContext = requestContext.ExecutionContext.ToExecutionContext();

            _executionContextManager
                .ForTenant(executionContext.Tenant)
                .ForCorrelation(executionContext.CorrelationId)
                .ForClaims(executionContext.Claims);

            var response = await Handler.Handle(request, token).ConfigureAwait(false);

            var responseContext = new ReverseCallResponseContext { CallId = requestContext.CallId };
            _protocol.SetResponseContextIn(responseContext, response);

            return response;
        }
    }
}