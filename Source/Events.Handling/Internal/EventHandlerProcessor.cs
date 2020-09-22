// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Handling.Internal
{
    /// <summary>
    /// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}" /> that can handle events.
    /// </summary>
    public class EventHandlerProcessor : EventProcessor<EventHandlerId, EventHandlerRegistrationRequest, HandleEventRequest, EventHandlerResponse>
    {
        readonly IEventHandler _eventHandler;
        readonly IEventProcessingConverter _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerProcessor"/> class.
        /// </summary>
        /// <param name="eventHandler">The <see cref="IEventHandler" />.</param>
        /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHandlerProcessor(
            IEventHandler eventHandler,
            IEventProcessingConverter converter,
            ILogger logger)
            : base("EventHandler", eventHandler.Identifier, logger)
        {
            _eventHandler = eventHandler;
            _converter = converter;
        }

        /// <inheritdoc/>
        public override EventHandlerRegistrationRequest RegistrationRequest
            {
                get
                {
                    var registrationRequest = new EventHandlerRegistrationRequest
                    {
                        EventHandlerId = _eventHandler.Identifier.ToProtobuf(),
                        ScopeId = _eventHandler.ScopeId.ToProtobuf(),
                        Partitioned = _eventHandler.Partitioned
                    };
                    registrationRequest.Types_.AddRange(_eventHandler.HandledEvents.Select(_ => _.ToProtobuf()).ToArray());
                    return registrationRequest;
                }
            }

        /// <inheritdoc/>
        protected override async Task<EventHandlerResponse> Process(HandleEventRequest request, ExecutionContext executionContext, CancellationToken cancellation)
        {
            var streamEvent = _converter.ToSDK(request.Event);
            var comittedEvent = streamEvent.Event;
            await _eventHandler
                .Handle(
                    comittedEvent.Content,
                    comittedEvent.EventType,
                    comittedEvent.GetEventContext(executionContext),
                    cancellation)
                .ConfigureAwait(false);
            return new EventHandlerResponse();
        }

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(HandleEventRequest request)
            => request.RetryProcessingState;

        /// <inheritdoc/>
        protected override EventHandlerResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new EventHandlerResponse { Failure = failure };
    }
}
