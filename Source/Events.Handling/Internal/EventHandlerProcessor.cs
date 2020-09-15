// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Internal
{
    /// <summary>
    /// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRegisterResponse, TRequest, TResponse}" /> that can handle events.
    /// </summary>
    public class EventHandlerProcessor : EventProcessor<EventHandlerId, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>
    {
        readonly IEventHandler _eventHandler;
        readonly IEventProcessingRequestConverter _processingRequestConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerProcessor"/> class.
        /// </summary>
        /// <param name="eventHandler">The <see cref="IEventHandler" />.</param>
        /// <param name="processingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHandlerProcessor(
            IEventHandler eventHandler,
            IEventProcessingRequestConverter processingRequestConverter,
            ILogger logger)
            : base(Kind, eventHandler.EventHandlerId, logger)
        {
            _eventHandler = eventHandler;
            _processingRequestConverter = processingRequestConverter;
        }

        /// <summary>
        /// Gets the <see cref="EventProcessorKind" />.
        /// </summary>
        public static EventProcessorKind Kind => "EventHandler";

        /// <inheritdoc/>
        protected override async Task<EventHandlerResponse> Process(HandleEventRequest request, CancellationToken cancellation)
        {
            var eventContext = _processingRequestConverter.GetEventContext(request.Event);
            var @event = _processingRequestConverter.GetCLREvent(request.Event);
            var eventType = request.Event.Event.Type.To<EventType>();

            await _eventHandler.Handle(@event, eventType, eventContext).ConfigureAwait(false);
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
