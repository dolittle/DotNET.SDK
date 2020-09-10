// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// An implementation of <see cref="IReverseCallClient{TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TClient">The type of generated gRPC client to use.</typeparam>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
    public class ReverseCallClient<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
        : IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse>
        where TClient : ClientBase<TClient>
        where TClientMessage : IMessage
        where TServerMessage : IMessage
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly ICanCallADuplexStreamingMethod<TClient, TClientMessage, TServerMessage> _method;
        readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _converter;
        readonly TimeSpan _pingInterval;
        readonly IPerformMethodCalls _caller;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;
        readonly IObservable<TConnectResponse> _observable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClient{TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to send to the server to start the reverse call protocol.</param>
        /// <param name="handler">The handler that will handle requests from the server.</param>
        /// <param name="method">The method that will be called on the server to initiate the reverse call.</param>
        /// <param name="converter">The converter that will be used to construct and deconstruct <typeparamref name="TClientMessage"/> and <typeparamref name="TServerMessage"/>.</param>
        /// <param name="pingInterval">The interval at which to request pings from the server to keep the reverse call alive.</param>
        /// <param name="caller">The caller that will be used to perform the method call.</param>
        /// <param name="executionContextManager">The execution context manager that will be used to set the execution context while handling requests from the server.</param>
        /// <param name="logger">The logger that will be used to log messages while performing the reverse call.</param>
        public ReverseCallClient(
            TConnectArguments arguments,
            IReverseCallHandler<TRequest, TResponse> handler,
            ICanCallADuplexStreamingMethod<TClient, TClientMessage, TServerMessage> method,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> converter,
            TimeSpan pingInterval,
            IPerformMethodCalls caller,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            Arguments = arguments;
            Handler = handler;
            _method = method;
            _converter = converter;
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
            => Observable.Create<TConnectResponse>((observer, token) =>
                {
                    var toServerMessages = new Subject<TClientMessage>();
                    var toClientMessages = _caller.Call(_method, toServerMessages, token);

                    var connectArguments = Arguments;
                    var connectContext = CreateReverseCallArgumentsContext();
                    _converter.SetConnectArgumentsContextIn(connectContext, connectArguments);
                    var connectMessage = _converter.CreateMessageFrom(connectArguments);

                    toServerMessages.OnNext(connectMessage);

                    var validMessages = toClientMessages.Skip(1).Where(MessageIsPingOrRequest).Timeout(_pingInterval * 3);
                    var pings = validMessages.Where(MessageIsPing);
                    var requests = validMessages.Where(MessageIsRequest);

                    var pongs = pings
                        .Select(_converter.GetPingFrom)
                        .Select(_ => new Pong())
                        .Select(_converter.CreateMessageFrom);

                    var responses = new Subject<TResponse>();
                    requests
                        .Select(_converter.GetRequestFrom)
                        .Where(RequestIsValid)
                        .Subscribe((request) =>
                            Task.Run(() => HandleRequest(request, responses, token), token));

                    pongs
                        .Merge(responses.Select(_converter.CreateMessageFrom))
                        .Subscribe(toServerMessages);

                    var connectResponse = toClientMessages.FirstAsync().Select(_converter.GetConnectResponseFrom);
                    var errorsAndCompletion = toClientMessages.Where(_ => false).Select(_converter.GetConnectResponseFrom);
                    connectResponse.Merge(errorsAndCompletion).Subscribe(observer);

                    return Task.CompletedTask;
                });

        ReverseCallArgumentsContext CreateReverseCallArgumentsContext()
            => new ReverseCallArgumentsContext
                {
                    HeadId = Guid.NewGuid().ToProtobuf(),
                    ExecutionContext = _executionContextManager.Current.ToProtobuf(),
                    PingInterval = Duration.FromTimeSpan(_pingInterval),
                };

        bool MessageIsPing(TServerMessage message)
            => _converter.GetPingFrom(message) != null;

        bool MessageIsRequest(TServerMessage message)
            => _converter.GetRequestFrom(message) != null;

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
            var context = _converter.GetRequestContextFrom(request);
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

        async Task HandleRequest(TRequest request, Subject<TResponse> responses, CancellationToken token)
        {
            var requestContext = _converter.GetRequestContextFrom(request);
            var executionContext = requestContext.ExecutionContext.ToExecutionContext();

            _executionContextManager
                .ForTenant(executionContext.Tenant)
                .ForCorrelation(executionContext.CorrelationId)
                .ForClaims(executionContext.Claims);

            var response = await Handler.Handle(request, token).ConfigureAwait(false);

            var responseContext = new ReverseCallResponseContext { CallId = requestContext.CallId };
            _converter.SetResponseContextIn(responseContext, response);

            responses.OnNext(response);
        }
    }
}
