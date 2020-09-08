// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Dolittle.SDK.Collections;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents a sequence of <see cref="UncommittedEvent"/>s that have not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvents : IReadOnlyList<UncommittedEvent>
    {
        readonly NullFreeList<UncommittedEvent> _events = new NullFreeList<UncommittedEvent>();

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the uncommitted sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <inheritdoc/>
        public UncommittedEvent this[int index] => _events[index];

        /// <summary>
        /// Appends an event to the uncommitted sequence.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId"/> of the <see cref="UncommittedEvent"/> to append.</param>
        /// <param name="event">The <see cref="IEvent"/> of the <see cref="UncommittedEvent"/> to append.</param>
        public void Append(EventSourceId eventSource, IEvent @event)
            => Append(new UncommittedEvent(eventSource, @event));

        /// <summary>
        /// Appends an event to the uncommitted sequence.
        /// </summary>
        /// <param name="event"><see cref="UncommittedEvent"/> to append.</param>
        public void Append(UncommittedEvent @event)
        {
            ThrowIfEventIsNull(@event);
            _events.Add(@event);
        }

        /// <inheritdoc/>
        public IEnumerator<UncommittedEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

        void ThrowIfEventIsNull(UncommittedEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }
    }
}
