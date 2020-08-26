// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Execution;

namespace Dolittle.Events
{
    /// <summary>
    /// Represent an Event that is committed to the Event Store.
    /// </summary>
    public class CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEvent"/> class.
        /// </summary>
        /// <param name="eventLogSequenceNumber">The event log sequence number of the Event.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" /> of the Event.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> in which the Event was committed.</param>
        /// <param name="event">An instance of the Event that was committed to the Event Store.</param>
        public CommittedEvent(
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSourceId,
            ExecutionContext executionContext,
            IEvent @event)
        {
            EventLogSequenceNumber = eventLogSequenceNumber;
            Occurred = occurred;
            ExecutionContext = executionContext;
            Event = @event;
            EventSource = eventSourceId;
        }

        /// <summary>
        /// Gets the event log sequence number of the Event.
        /// </summary>
        public EventLogSequenceNumber EventLogSequenceNumber { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.
        /// </summary>
        public DateTimeOffset Occurred { get; }

        /// <summary>
        /// Gets the Event Source that this Event was applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

        /// <summary>
        /// Gets the <see cref="ExecutionContext"/> in which the Event was committed.
        /// </summary>
        public ExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Gets an instance of the Event that was committed to the Event Store.
        /// </summary>
        public IEvent Event { get; }
    }
}