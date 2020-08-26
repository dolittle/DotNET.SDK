// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Events.Processing.Internal;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services.Clients;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;
using Artifact = Dolittle.Artifacts.Contracts.Artifact;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Events.Handling.Internal
{
    /// <summary>
    /// Implementation of <see cref="EventProcessor{TIdentifier, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> for Event Handlers.
    /// </summary>
    /// <typeparam name="TEventType">The event type that the handler can handle.</typeparam>
    public class EventHandlerProcessor<TEventType> : EventProcessor<EventHandlerId, EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>
        where TEventType : IEvent
    {
        readonly ScopeId _scope;
        readonly bool _partitioned;
        readonly IEventHandler<TEventType> _handler;
        readonly EventHandlersClient _client;
        readonly IReverseCallClients _reverseCallClients;
        readonly IEventProcessingCompletion _completion;
        readonly IArtifactTypeMap _artifacts;
        readonly IEventConverter _converter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerProcessor{TEventType}"/> class.
        /// </summary>
        /// <param name="handlerId">The unique <see cref="EventHandlerId"/> for the event handler.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the scope in the Event Store where the event handler will run.</param>
        /// <param name="partitioned">Whether the event handler should create a partitioned stream or not.</param>
        /// <param name="handler">The <see cref="IEventHandler{TEventType}"/> that will be called to handle incoming events.</param>
        /// <param name="client">The <see cref="EventHandlersClient"/> to use to connect to the Runtime.</param>
        /// <param name="reverseCallClients">The <see cref="IReverseCallClients"/> to use for creating instances of <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.</param>
        /// <param name="completion">The <see cref="IEventProcessingCompletion"/> to use for notifying of event handling completion.</param>
        /// <param name="artifacts">The <see cref="IArtifactTypeMap"/> to use for converting event types to artifacts.</param>
        /// <param name="converter">The <see cref="IEventConverter"/> to use to convert events.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public EventHandlerProcessor(
            EventHandlerId handlerId,
            ScopeId scope,
            bool partitioned,
            IEventHandler<TEventType> handler,
            EventHandlersClient client,
            IReverseCallClients reverseCallClients,
            IEventProcessingCompletion completion,
            IArtifactTypeMap artifacts,
            IEventConverter converter,
            ILogger logger)
            : base(logger)
        {
            Identifier = handlerId;
            _scope = scope;
            _partitioned = partitioned;
            _handler = handler;
            _client = client;
            _reverseCallClients = reverseCallClients;
            _completion = completion;
            _artifacts = artifacts;
            _converter = converter;
            _logger = logger;
        }

        /// <inheritdoc/>
        protected override string Kind => "event handler";

        /// <inheritdoc/>
        protected override EventHandlerId Identifier { get; }

        /// <inheritdoc/>
        protected override IReverseCallClient<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> CreateClient()
            => _reverseCallClients.GetFor<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>(
                () => _client.Connect(),
                (message, arguments) => message.RegistrationRequest = arguments,
                message => message.RegistrationResponse,
                message => message.HandleRequest,
                (message, response) => message.HandleResult = response,
                (arguments, context) => arguments.CallContext = context,
                request => request.CallContext,
                (response, context) => response.CallContext = context,
                messsage => messsage.Ping,
                (message, pong) => message.Pong = pong,
                TimeSpan.FromSeconds(5));

        /// <inheritdoc/>
        protected override EventHandlerResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new EventHandlerResponse
            {
                Failure = failure,
            };

        /// <inheritdoc/>
        protected override Failure GetFailureFromRegisterResponse(EventHandlerRegistrationResponse response)
            => response.Failure;

        /// <inheritdoc/>
        protected override EventHandlerRegistrationRequest GetRegisterArguments()
        {
            var request = new EventHandlerRegistrationRequest
            {
                EventHandlerId = Identifier.ToProtobuf(),
                ScopeId = _scope.ToProtobuf(),
                Partitioned = _partitioned,
            };

            foreach (var eventType in _handler.HandledEventTypes)
            {
                var artifact = _artifacts.GetArtifactFor(eventType);
                request.Types_.Add(new Artifact
                {
                    Id = artifact.Id.ToProtobuf(),
                    Generation = artifact.Generation,
                });
            }

            return request;
        }

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingState(HandleEventRequest request)
            => request.RetryProcessingState;

        /// <inheritdoc/>
        protected override async Task<EventHandlerResponse> Handle(HandleEventRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var comitted = _converter.ToSDK(request.Event.Event);
                if (comitted.Event is TEventType typedEvent)
                {
                    await _handler.Handle(typedEvent, comitted.DeriveContext()).ConfigureAwait(false);
                    return new EventHandlerResponse();
                }

                throw new EventHandlerDoesNotHandleEvent(typeof(TEventType));
            }
            finally
            {
                try
                {
                    var comitted = _converter.ToSDK(request.Event.Event);
                    var correlationId = comitted.ExecutionContext.CorrelationId;
                    var eventType = comitted.Event.GetType();
                    _completion.EventHandlerCompletedForEvent(correlationId, Identifier, eventType);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error notifying waiters of event handler completion");
                }
            }
        }
    }
}
