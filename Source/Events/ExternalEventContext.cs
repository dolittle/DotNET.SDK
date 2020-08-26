// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Execution;

namespace Dolittle.Events
{
    /// <summary>
    /// Represents the context in which an external event occured in.
    /// </summary>
    public class ExternalEventContext : EventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalEventContext"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber">sequence number</see> that uniquely identifies the event in the event log which it was received.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> that the event was committed to.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset"/> when the event was committed to the <see cref="IEventStore"/>.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> in which the event was committed to the <see cref="IEventStore"/>.</param>
        /// <param name="externalEventLogSequenceNumber">The <see cref="EventLogSequenceNumber">sequence number</see> that uniquely identifies the event in the event log which it was committed (the external Microservice event log).</param>
        /// <param name="received">The <see cref="DateTimeOffset"/> when the Event was received over the Event Horizon.</param>
        public ExternalEventContext(
            EventLogSequenceNumber sequenceNumber,
            EventSourceId eventSourceId,
            DateTimeOffset occurred,
            ExecutionContext executionContext,
            EventLogSequenceNumber externalEventLogSequenceNumber,
            DateTimeOffset received)
            : base(
                sequenceNumber,
                eventSourceId,
                occurred,
                executionContext,
                new EventIdentifier(executionContext.Microservice, executionContext.Tenant, externalEventLogSequenceNumber))
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