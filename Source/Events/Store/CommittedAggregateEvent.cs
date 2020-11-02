// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represent an Event that was applied to an Event Source by an Aggregate Root and is committed to the Event Store.
    /// </summary>
    public class CommittedAggregateEvent : CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedAggregateEvent"/> class.
        /// </summary>
        /// <param name="eventLogSequenceNumber">The event log sequence number of the Event.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the Event.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the Aggregate Root.</param>
        /// <param name="aggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that applied the Event.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> in which the Event was committed.</param>
        /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
        /// <param name="content">The content of the Event.</param>
        /// <param name="isPublic">Whether the event is public or not.</param>
        public CommittedAggregateEvent(
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion aggregateRootVersion,
            ExecutionContext executionContext,
            EventType eventType,
            object content,
            bool isPublic)
            : base(eventLogSequenceNumber, occurred, eventSourceId, executionContext, eventType, content, isPublic)
        {
            ThrowIfAggregateRootIdIsNull(aggregateRootId);
            ThrowIfAggregateRootVersionIsNull(aggregateRootVersion);

            AggregateRoot = aggregateRootId;
            AggregateRootVersion = aggregateRootVersion;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the Aggregate Root that applied the Event to the Event Source.
        /// </summary>
        public AggregateRootId AggregateRoot { get; }

        /// <summary>
        /// Gets the version of the Aggregate Root that applied the Event.
        /// </summary>
        public AggregateRootVersion AggregateRootVersion { get; }

        void ThrowIfAggregateRootIdIsNull(AggregateRootId aggregateRootId)
        {
            if (aggregateRootId == null) throw new AggregateRootIdCannotBeNull();
        }

        void ThrowIfAggregateRootVersionIsNull(AggregateRootVersion aggregateRootVersion)
        {
            if (aggregateRootVersion == null) throw new AggregateRootVersionCannotBeNull();
        }
    }
}
