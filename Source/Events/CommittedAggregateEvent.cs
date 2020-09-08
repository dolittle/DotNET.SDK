// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represent an Event that was applied to an Event Source by an <see cref="AggregateRoot"/> and is committed to the Event Store.
    /// </summary>
    public class CommittedAggregateEvent : CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedAggregateEvent"/> class.
        /// </summary>
        /// <param name="eventLogSequenceNumber">The event log sequence number of the Event.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" /> of the Event.</param>
        /// <param name="aggregateRoot">The <see cref="Type"/> of the Aggregate Root that applied the Event to the Event Source.</param>
        /// <param name="aggregateRootVersion">The version of the <see cref="AggregateRoot"/> that applied the Event.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> in which the Event was committed.</param>
        /// <param name="event">An instance of the Event that was committed to the Event Store.</param>
        public CommittedAggregateEvent(
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSourceId,
            Type aggregateRoot,
            AggregateRootVersion aggregateRootVersion,
            ExecutionContext executionContext,
            IEvent @event)
            : base(eventLogSequenceNumber, occurred, eventSourceId, executionContext, @event)
        {
            AggregateRoot = aggregateRoot;
            AggregateRootVersion = aggregateRootVersion;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the Aggregate Root that applied the Event to the Event Source.
        /// </summary>
        public Type AggregateRoot { get; }

        /// <summary>
        /// Gets the version of the <see cref="AggregateRoot"/> that applied the Event.
        /// </summary>
        public AggregateRootVersion AggregateRootVersion { get; }
    }
}
