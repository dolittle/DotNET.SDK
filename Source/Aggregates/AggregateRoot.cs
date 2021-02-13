// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Represents the aggregate root.
    /// </summary>
    public class AggregateRoot
    {
        readonly IList<UncommittedAggregateEvent> _uncommittedEvents;
        readonly IEventTypes _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="eventSourceId">The <see cref="Events.EventSourceId" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        public AggregateRoot(EventSourceId eventSourceId, IEventTypes eventTypes)
        {
            EventSourceId = eventSourceId;
            Version = AggregateRootVersion.Initial;
            _uncommittedEvents = new List<UncommittedAggregateEvent>();
            _eventTypes = eventTypes;
        }

        /// <summary>
        /// Gets the current <see cref="AggregateRootVersion" />.
        /// </summary>
        public AggregateRootVersion Version { get; private set; }

        /// <summary>
        /// Gets the <see cref="Events.EventSourceId" /> that the <see cref="AggregateRoot" /> applies events to.
        /// </summary>
        public EventSourceId EventSourceId { get; }

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}" /> of events to commit.
        /// </summary>
        public IEnumerable<UncommittedAggregateEvent> UncommittedEvents => _uncommittedEvents;

        /// <summary>
        /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
        /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(System.Action{TAggregate})" /> is invoked on the <see cref="AggregateRoot" />.
        /// </summary>
        /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
        /// <param name="event">The event to apply.</param>
        /// <param name="isPublic">Whether to apply a public event.</param>
        public void Apply(object @event, bool isPublic = false)
        {
            if (@event == null) throw new EventContentCannotBeNull();
            Apply(@event, _eventTypes.GetFor(@event.GetType()), isPublic);
        }

        /// <summary>
        /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
        /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(System.Action{TAggregate})" /> is invoked on the <see cref="AggregateRoot" />.
        /// </summary>
        /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
        /// <param name="event">The event to apply.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId" />.</param>
        /// <param name="isPublic">Whether to apply a public event.</param>
        public void Apply(object @event, EventTypeId eventTypeId, bool isPublic = false)
            => Apply(@event, new EventType(eventTypeId), isPublic);

        /// <summary>
        /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
        /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(System.Action{TAggregate})" /> is invoked on the <see cref="AggregateRoot" />.
        /// </summary>
        /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
        /// <param name="event">The event to apply.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event type.</param>
        /// <param name="generation">The <see cref="Generation" /> of the event type.</param>
        /// <param name="isPublic">Whether to apply a public event.</param>
        public void Apply(object @event, EventTypeId eventTypeId, Generation generation, bool isPublic = false)
            => Apply(@event, new EventType(eventTypeId, generation), isPublic);

        /// <summary>
        /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
        /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(System.Action{TAggregate})" /> is invoked on the <see cref="AggregateRoot" />.
        /// </summary>
        /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
        /// <param name="event">The event to apply.</param>
        /// <param name="eventType">The <see cref="EventType" />.</param>
        /// <param name="isPublic">Whether to apply a public event.</param>
        public void Apply(object @event, EventType eventType, bool isPublic = false)
        {
            ThrowIfWrongEventType(@event, eventType);
            _uncommittedEvents.Add(new UncommittedAggregateEvent(eventType, @event, isPublic));
            Version++;
            InvokeOnMethod(@event);
        }

        /// <summary>
        /// Re-apply events from the Event Store.
        /// </summary>
        /// <param name="events">Sequence that contains the events to re-apply.</param>
        public virtual void ReApply(CommittedAggregateEvents events)
        {
            ThrowIfEventWasAppliedToOtherEventSource(events);
            ThrowIfEventWasAppliedByOtherAggregateRoot(events);

            foreach (var @event in events)
            {
                ThrowIfAggreggateRootVersionIsOutOfOrder(@event);
                Version++;
                if (!this.IsStateless()) InvokeOnMethod(@event.Content);
            }
        }

        void InvokeOnMethod(object @event)
        {
            if (this.TryGetOnMethod(@event, out var handleMethod))
            {
                handleMethod.Invoke(this, new[] { @event });
            }
        }

        void ThrowIfWrongEventType(object @event, EventType eventType)
        {
            var typeOfEvent = @event.GetType();
            if (_eventTypes.HasFor(typeOfEvent))
            {
                var associatedEventType = _eventTypes.GetFor(typeOfEvent);
                if (eventType != associatedEventType)
                    throw new ProvidedEventTypeDoesNotMatchEventTypeFromAttribute(eventType, associatedEventType, typeOfEvent);
            }
        }

        void ThrowIfAggreggateRootVersionIsOutOfOrder(CommittedAggregateEvent @event)
        {
            if (@event.AggregateRootVersion != Version) throw new AggregateRootVersionIsOutOfOrder(@event.AggregateRootVersion, Version);
        }

        void ThrowIfEventWasAppliedByOtherAggregateRoot(CommittedAggregateEvents events)
        {
            var aggregateRootId = this.GetAggregateRootId();
            if (events.AggregateRoot != this.GetAggregateRootId()) throw new EventWasAppliedByOtherAggregateRoot(events.AggregateRoot, aggregateRootId);
        }

        void ThrowIfEventWasAppliedToOtherEventSource(CommittedAggregateEvents events)
        {
            if (events.EventSource != EventSourceId) throw new EventWasAppliedToOtherEventSource(events.EventSource, EventSourceId);
        }
    }
}