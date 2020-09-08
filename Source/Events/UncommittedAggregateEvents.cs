// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Dolittle.SDK.Collections;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents a sequence of <see cref="IEvent"/>s applied by an AggregateRoot to an Event Source that have not been committed to the Event Store.
    /// </summary>
    public class UncommittedAggregateEvents : IReadOnlyList<IEvent>
    {
        readonly NullFreeList<IEvent> _events = new NullFreeList<IEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedAggregateEvents"/> class.
        /// </summary>
        /// <param name="eventSource">The Event Source that the uncommitted events was applied to.</param>
        /// <param name="aggregateRoot">The <see cref="Type"/> of the Aggregate Root that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        public UncommittedAggregateEvents(EventSourceId eventSource, Type aggregateRoot, AggregateRootVersion expectedAggregateRootVersion)
        {
            EventSource = eventSource;
            AggregateRoot = aggregateRoot;
            ExpectedAggregateRootVersion = expectedAggregateRootVersion;
        }

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the uncommitted sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <summary>
        /// Gets the <see cref="Type"/> of the Aggregate Root that applied the events to the Event Source.
        /// </summary>
        public Type AggregateRoot { get; }

        /// <summary>
        /// Gets the Event Source that the uncommitted events was applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

        /// <summary>
        /// Gets the <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.
        /// The events can only be committed to the Event Store if the version of Aggregate Root has not changed.
        /// </summary>
        public AggregateRootVersion ExpectedAggregateRootVersion { get; }

        /// <inheritdoc/>
        public IEvent this[int index] => _events[index];

        /// <summary>
        /// Appends an event to the uncommitted sequence.
        /// </summary>
        /// <param name="event"><see cref="IEvent"/> to append.</param>
        public void Append(IEvent @event)
        {
            ThrowIfEventIsNull(@event);
            _events.Add(@event);
        }

        /// <inheritdoc/>
        public IEnumerator<IEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

        void ThrowIfEventIsNull(IEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }
    }
}
