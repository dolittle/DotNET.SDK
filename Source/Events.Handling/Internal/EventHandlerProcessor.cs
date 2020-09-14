// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;

namespace Dolittle.SDK.Events.Handling.Internal
{
    /// <summary>
    /// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRegisterResponse, TRequest, TResponse}" /> that can handle events.
    /// </summary>
    public class EventHandlerProcessor : EventProcessor<EventHandlerId, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>
    {
        readonly IEventHandler _eventHandler;
        readonly ICreateReverseCallClients _reverseCallClientsCreator;
        readonly IEventProcessingRequestConverter _processingRequestConverter;
        EventHandlerRegistrationRequest _registrationRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerProcessor"/> class.
        /// </summary>
        /// <param name="eventHandler">The <see cref="IEventHandler" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="processingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHandlerProcessor(
            IEventHandler eventHandler,
            ICreateReverseCallClients reverseCallClientsCreator,
            IEventProcessingRequestConverter processingRequestConverter,
            ILogger logger)
            : base("EventHandler", eventHandler.EventHandlerId, logger)
        {
            _eventHandler = eventHandler;
            _reverseCallClientsCreator = reverseCallClientsCreator;
            _processingRequestConverter = processingRequestConverter;
        }

        /// <inheritdoc/>
        protected override EventHandlerRegistrationRequest RegisterArguments
        {
            get
            {
                if (_registrationRequest == default)
                {
                    _registrationRequest = new EventHandlerRegistrationRequest
                    {
                        EventHandlerId = Identifier.ToProtobuf(),
                        ScopeId = _eventHandler.ScopeId.ToProtobuf(),
                        Partitioned = _eventHandler.Partitioned
                    };
                    _registrationRequest.Types_.AddRange(_eventHandler.HandledEvents.Select(_ => _.ToProtobuf()).ToArray());
                }

                return _registrationRequest;
            }
        }

        /// <inheritdoc/>
        public override async Task<EventHandlerResponse> Handle(HandleEventRequest request, CancellationToken cancellation)
        {
            var eventContext = _processingRequestConverter.GetEventContext(request.Event);
            var @event = _processingRequestConverter.GetCLREvent(request.Event);
            var eventType = request.Event.Event.Type.To<EventType>();

            await _eventHandler.Handle(@event, eventType, eventContext).ConfigureAwait(false);
            return new EventHandlerResponse();
        }

        /// <inheritdoc/>
        protected override IReverseCallClient<EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> CreateClient(
            EventHandlerRegistrationRequest registerArguments,
            Func<HandleEventRequest, CancellationToken, Task<EventHandlerResponse>> callback,
            uint pingTimeout,
            CancellationToken cancellation)
            => _reverseCallClientsCreator.Create(
                RegisterArguments,
                this,
                new DuplexStreamingMethodCaller(),
                new ReverseCallMessageConverter());

        /// <inheritdoc/>
        protected override Failure GetFailureFromRegisterResponse(EventHandlerRegistrationResponse response) => response.Failure;

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(HandleEventRequest request) => request.RetryProcessingState;

        /// <inheritdoc/>
        protected override EventHandlerResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new EventHandlerResponse { Failure = failure };

        class DuplexStreamingMethodCaller : ICanCallADuplexStreamingMethod<EventHandlersClient, EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage>
        {
            public AsyncDuplexStreamingCall<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage> Call(Channel channel, CallOptions callOptions)
                => new EventHandlersClient(channel).Connect(callOptions);
        }

        class ReverseCallMessageConverter : IConvertReverseCallMessages<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>
        {
            public EventHandlerClientToRuntimeMessage CreateMessageFrom(EventHandlerRegistrationRequest arguments)
                => new EventHandlerClientToRuntimeMessage { RegistrationRequest = arguments };

            public EventHandlerClientToRuntimeMessage CreateMessageFrom(Pong pong)
                => new EventHandlerClientToRuntimeMessage { Pong = pong };

            public EventHandlerClientToRuntimeMessage CreateMessageFrom(EventHandlerResponse response)
                => new EventHandlerClientToRuntimeMessage { HandleResult = response };

            public EventHandlerRegistrationResponse GetConnectResponseFrom(EventHandlerRuntimeToClientMessage message)
                => message.RegistrationResponse;

            public Ping GetPingFrom(EventHandlerRuntimeToClientMessage message)
                => message.Ping;

            public ReverseCallRequestContext GetRequestContextFrom(HandleEventRequest message)
                => message.CallContext;

            public HandleEventRequest GetRequestFrom(EventHandlerRuntimeToClientMessage message)
                => message.HandleRequest;

            public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, EventHandlerRegistrationRequest arguments)
                => arguments.CallContext = context;

            public void SetResponseContextIn(ReverseCallResponseContext context, EventHandlerResponse response)
                => response.CallContext = context;
        }
    }
}