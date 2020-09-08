// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents an <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRegisterResponse, TRequest, TResponse}" /> for an event filter.
    /// </summary>
    /// <typeparam name="TRegisterArguments">The <see cref="System.Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TResponse">The <see cref="System.Type" /> of the response.</typeparam>
    public abstract class FilterEventProcessor<TRegisterArguments, TResponse> : EventProcessor<FilterId, TRegisterArguments, FilterRegistrationResponse, FilterEventRequest, TResponse>
        where TRegisterArguments : class
        where TResponse : class
    {
        readonly EventTypes _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}"/> class.
        /// </summary>
        /// <param name="kind">The kind of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" />.</param>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        protected FilterEventProcessor(string kind, FilterId filterId, EventTypes eventTypes, ILogger logger)
            : base(kind, filterId, logger)
        {
            _eventTypes = eventTypes;
        }

        /// <inheritdoc/>
        public override Task<TResponse> Handle(FilterEventRequest request, CancellationToken cancellation)
        {
            var pbEvent = request.Event;
            if (pbEvent == default) throw new MissingEventInformation("No event in FilterEventRequest");
            var eventContext = CreateEventContext(pbEvent);
            var eventType = GetEventTypeOrThrow(pbEvent);
            var clrEventType = _eventTypes.GetTypeFor(eventType);
            var @event = JsonSerializer.Deserialize(pbEvent.Content, clrEventType);

            return Filter(@event, eventContext);
        }

        /// <summary>
        /// Filters an event.
        /// </summary>
        /// <param name="event">The event to filter.</param>
        /// <param name="context">The <see cref="EventContext" />.</param>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns a <typeparamref name="TResponse"/>.</returns>
        protected abstract Task<TResponse> Filter(object @event, EventContext context);

        /// <inheritdoc/>
        protected override Failure GetFailureFromRegisterResponse(FilterRegistrationResponse response) => response.Failure;

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(FilterEventRequest request) => request.RetryProcessingState;

        EventContext CreateEventContext(PbCommittedEvent pbEvent)
        {
            var sequenceNumber = pbEvent.EventLogSequenceNumber;
            var eventSourceId = GetEventSourceIdOrThrow(pbEvent);
            var executionContext = GetExecutionContextOrThrow(pbEvent);
            var occurred = GetOccurredOrThrow(pbEvent);

            return new EventContext(sequenceNumber, eventSourceId, occurred, executionContext);
        }

        EventSourceId GetEventSourceIdOrThrow(PbCommittedEvent pbEvent)
        {
            if (pbEvent.EventSourceId == default) throw new MissingEventInformation("EventSourceId");
            return pbEvent.EventSourceId.To<EventSourceId>();
        }

        Execution.ExecutionContext GetExecutionContextOrThrow(PbCommittedEvent pbEvent)
        {
            if (pbEvent.ExecutionContext == default) throw new MissingEventInformation("ExecutionContext");
            return pbEvent.ExecutionContext.ToExecutionContext();
        }

        System.DateTimeOffset GetOccurredOrThrow(PbCommittedEvent pbEvent)
        {
            if (pbEvent.Occurred == default) throw new MissingEventInformation("Occurred");
            return pbEvent.Occurred.ToDateTimeOffset();
        }

        EventType GetEventTypeOrThrow(PbCommittedEvent pbEvent)
        {
            if (pbEvent.Type == default) throw new MissingEventInformation("ExecutionContext");
            return pbEvent.Type.To<EventType>();
        }
    }
}