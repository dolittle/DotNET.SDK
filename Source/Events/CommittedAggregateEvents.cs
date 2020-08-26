// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;

namespace Dolittle.Events
{
    /// <summary>
    /// Represents a sequence of <see cref="IEvent"/>s applied by an AggregateRoot to an Event Source that have been committed to the Event Store.
    /// </summary>
    public class CommittedAggregateEvents : IReadOnlyList<CommittedAggregateEvent>
    {
        readonly NullFreeList<CommittedAggregateEvent> _events;
        readonly AggregateRootVersion _nextAggregateRootVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedAggregateEvents"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId"/> that the Events were applied to.</param>
        /// <param name="aggregateRoot">The <see cref="Type"/> of the Aggregate Root that applied the Events to the Event Source.</param>
        /// <param name="events">The <see cref="CommittedAggregateEvent">events</see>.</param>
        public CommittedAggregateEvents(EventSourceId eventSource, Type aggregateRoot, IReadOnlyList<CommittedAggregateEvent> events)
        {
            EventSource = eventSource;
            AggregateRoot = aggregateRoot;

            for (var i = 0; i < events.Count; i++)
            {
                if (i == 0) _nextAggregateRootVersion = events[0].AggregateRootVersion;
                var @event = events[i];
                ThrowIfEventIsNull(@event);
                ThrowIfEventWasAppliedToOtherEventSource(@event);
                ThrowIfEventWasAppliedByOtherAggregateRoot(@event);
                ThrowIfAggreggateRootVersionIsOutOfOrder(@event);
                if (i > 0) ThrowIfEventLogVersionIsOutOfOrder(@event, events[i - 1]);
                _nextAggregateRootVersion++;
            }

            _events = new NullFreeList<CommittedAggregateEvent>(events);
        }

        /// <summary>
        /// Gets the Event Source that the Events were applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the Aggregate Root that applied the Events to the Event Source.
        /// </summary>
        public Type AggregateRoot { get; }

        /// <summary>
        /// Gets the version of the <see cref="AggregateRoot"/> after all the Events was applied.
        /// </summary>
        public AggregateRootVersion AggregateRootVersion => _events.Count == 0 ? AggregateRootVersion.Initial : _events.Last().AggregateRootVersion;

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the committed sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <inheritdoc/>
        public CommittedAggregateEvent this[int index] => _events[index];

        /// <inheritdoc/>
        public IEnumerator<CommittedAggregateEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

        void ThrowIfEventIsNull(CommittedAggregateEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }

        void ThrowIfEventWasAppliedToOtherEventSource(CommittedAggregateEvent @event)
        {
            if (@event.EventSource != EventSource) throw new EventWasAppliedToOtherEventSource(@event.EventSource, EventSource);
        }

        void ThrowIfEventWasAppliedByOtherAggregateRoot(CommittedAggregateEvent @event)
        {
            if (@event.AggregateRoot != AggregateRoot) throw new EventWasAppliedByOtherAggregateRoot(@event.AggregateRoot, AggregateRoot);
        }

        void ThrowIfAggreggateRootVersionIsOutOfOrder(CommittedAggregateEvent @event)
        {
            if (@event.AggregateRootVersion != _nextAggregateRootVersion) throw new AggregateRootVersionIsOutOfOrder(@event.AggregateRootVersion, _nextAggregateRootVersion);
        }

        void ThrowIfEventLogVersionIsOutOfOrder(CommittedAggregateEvent @event, CommittedAggregateEvent previousEvent)
        {
            if (@event.EventLogSequenceNumber <= previousEvent.EventLogSequenceNumber) throw new EventLogSequenceNumberIsOutOfOrder(@event.EventLogSequenceNumber, previousEvent.EventLogSequenceNumber);
        }
    }
}