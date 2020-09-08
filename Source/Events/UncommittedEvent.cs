// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an <see cref="IEvent"/> that has not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEvent"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" /> of the Event.</param>
        /// <param name="event">An instance of the Event to be committed to the Event Store.</param>
        public UncommittedEvent(EventSourceId eventSource, IEvent @event)
        {
            ThrowIfEventIsNull(@event);
            EventSource = eventSource;
            Event = @event;
        }

        /// <summary>
        /// Gets the Event Source that this Event was applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

        /// <summary>
        /// Gets an instance of the Event to be committed to the Event Store.
        /// </summary>
        public IEvent Event { get; }

        void ThrowIfEventIsNull(IEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }
    }
}
