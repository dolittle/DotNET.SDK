// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Dolittle.Artifacts.Contracts;
using Dolittle.Execution.Contracts;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using PbStreamEvent = Dolittle.Runtime.Events.Processing.Contracts.StreamEvent;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessingRequestConverter" />.
    /// </summary>
    public class EventProcessingRequestConverter : IEventProcessingRequestConverter
    {
        readonly IEventTypes _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessingRequestConverter"/> class.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        public EventProcessingRequestConverter(IEventTypes eventTypes)
        {
            _eventTypes = eventTypes;
        }

        /// <inheritdoc/>
        public object GetCLREvent(PbCommittedEvent @event)
        {
            var pbEvent = GetEventOrThrow(@event);
            var eventType = GetEventTypeOrThrow(pbEvent.Type);
            var clrEventType = _eventTypes.GetTypeFor(eventType);

            return JsonSerializer.Deserialize(pbEvent.Content, clrEventType);
        }

        /// <inheritdoc/>
        public object GetCLREvent(PbStreamEvent @event)
            => GetCLREvent(GetEventOrThrow(@event));

        /// <inheritdoc/>
        public T GetCLREvent<T>(PbCommittedEvent @event)
            where T : class
        {
            var pbEvent = GetEventOrThrow(@event);
            var eventType = GetEventTypeOrThrow(pbEvent.Type);
            var associatedEventType = _eventTypes.GetFor<T>();
            if (eventType != associatedEventType) throw new EventTypeDoesNotMatchAssociatedEventType(eventType, associatedEventType, typeof(T));
            return JsonSerializer.Deserialize<T>(pbEvent.Content);
        }

        /// <inheritdoc/>
        public T GetCLREvent<T>(PbStreamEvent @event)
            where T : class
            => GetCLREvent<T>(GetEventOrThrow(@event));

        /// <inheritdoc/>
        public EventContext GetEventContext(PbCommittedEvent @event)
            => new EventContext(
                @event.EventLogSequenceNumber,
                GetEventSourceIdOrThrow(@event.EventSourceId),
                GetOccurredOrThrow(@event.Occurred),
                GetExecutionContextOrThrow(@event.ExecutionContext));

        /// <inheritdoc/>
        public EventContext GetEventContext(PbStreamEvent @event)
            => new EventContext(
                @event.Event.EventLogSequenceNumber,
                GetEventSourceIdOrThrow(@event.Event.EventSourceId),
                GetOccurredOrThrow(@event.Event.Occurred),
                GetExecutionContextOrThrow(@event.Event.ExecutionContext));

        PbCommittedEvent GetEventOrThrow(PbCommittedEvent @event)
        {
            if (@event == default) throw new MissingEventInformation("committed event");
            return @event;
        }

        PbCommittedEvent GetEventOrThrow(PbStreamEvent @event)
        {
            if (@event.Event == default) throw new MissingEventInformation("committed event");
            return @event.Event;
        }

        EventSourceId GetEventSourceIdOrThrow(Uuid eventSourceId)
        {
            if (eventSourceId == default) throw new MissingEventInformation("EventSourceId");
            return eventSourceId.To<EventSourceId>();
        }

        Execution.ExecutionContext GetExecutionContextOrThrow(ExecutionContext executionContext)
        {
            if (executionContext == default) throw new MissingEventInformation("ExecutionContext");
            return executionContext.ToExecutionContext();
        }

        System.DateTimeOffset GetOccurredOrThrow(Timestamp occurred)
        {
            if (occurred == default) throw new MissingEventInformation("Occurred");
            return occurred.ToDateTimeOffset();
        }

        EventType GetEventTypeOrThrow(Artifact artifact)
        {
            if (artifact == default) throw new MissingEventInformation("ExecutionContext");
            return artifact.To<EventType>();
        }
    }
}