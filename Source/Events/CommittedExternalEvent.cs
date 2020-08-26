// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Execution;

namespace Dolittle.Events
{
    /// <summary>
    /// Represents an Event that was received over the Event Horizon from an external Microservice.
    /// </summary>
    public class CommittedExternalEvent : CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedExternalEvent"/> class.
        /// </summary>
        /// <param name="eventLogSequenceNumber">The event log sequence number of the Event.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset"/> when the Event was committed to the Event Store.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the Event.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> in which the Event was committed.</param>
        /// <param name="externalEventLogSequenceNumber">The event log sequence number of the Event in the event log of the external Microservice that committed the event.</param>
        /// <param name="received">The <see cref="DateTimeOffset"/> when the Event was received over the Event Horizon.</param>
        /// <param name="event">An instance of the Event that was committed to the Event Store.</param>
        public CommittedExternalEvent(
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSourceId,
            ExecutionContext executionContext,
            EventLogSequenceNumber externalEventLogSequenceNumber,
            DateTimeOffset received,
            IEvent @event)
            : base(eventLogSequenceNumber, occurred, eventSourceId, executionContext, @event)
        {
            ExternalEventLogSequenceNumber = externalEventLogSequenceNumber;
            Received = received;
        }

        /// <summary>
        /// Gets the event log sequence number of the Event in the event log of the external Microservice that committed the event.
        /// </summary>
        public EventLogSequenceNumber ExternalEventLogSequenceNumber { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset"/> when the Event was received over the Event Horizon.
        /// </summary>
        public DateTimeOffset Received { get; }
    }
}