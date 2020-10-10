// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represent an Event that is committed to the Event Store through the Event Horizon.
    /// </summary>
    public class CommittedExternalEvent : CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedExternalEvent"/> class.
        /// </summary>
        /// <param name="eventLogSequenceNumber">The event log sequence number of the Event.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the Event.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> in which the Event was committed.</param>
        /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
        /// <param name="content">The content of the Event.</param>
        /// <param name="isPublic">Whether the event is public or not.</param>
        /// <param name="externalEventLogSequenceNumber">The event log sequence number of the Event in the event log of the external Microservice that committed the event.</param>
        /// <param name="externalEventReceived">The <see cref="DateTimeOffset"/> when the Event was received over the Event Horizon.</param>
        public CommittedExternalEvent(
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSourceId,
            ExecutionContext executionContext,
            EventType eventType,
            object content,
            bool isPublic,
            EventLogSequenceNumber externalEventLogSequenceNumber,
            DateTimeOffset externalEventReceived)
            : base(
                eventLogSequenceNumber,
                occurred,
                eventSourceId,
                executionContext,
                eventType,
                content,
                isPublic)
        {
            ExternalEventLogSequenceNumber = externalEventLogSequenceNumber;
            ExternalEventReceived = externalEventReceived;
        }

        /// <summary>
        /// Gets the event log sequence number of the Event from the external Microservice.
        /// </summary>
        public EventLogSequenceNumber ExternalEventLogSequenceNumber { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset" /> when the Event was received in the external Microservice.
        /// </summary>
        public DateTimeOffset ExternalEventReceived { get; }
    }
}
