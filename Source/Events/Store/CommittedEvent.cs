// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events.Store
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
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the Event.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> in which the Event was committed.</param>
        /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
        /// <param name="content">The content of the Event.</param>
        /// <param name="isPublic">Whether the event is public or not.</param>
        public CommittedEvent(
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSourceId,
            ExecutionContext executionContext,
            EventType eventType,
            object content,
            bool isPublic)
        {
            EventLogSequenceNumber = eventLogSequenceNumber;
            Occurred = occurred;
            EventSource = eventSourceId;
            ExecutionContext = executionContext;
            EventType = eventType;
            Content = content;
            IsPublic = isPublic;
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
        /// Gets the <see cref="EventType"/> in which the Event is associated with.
        /// </summary>
        public EventType EventType { get; }

        /// <summary>
        /// Gets the content of the Event.
        /// </summary>
        public object Content { get; }

        /// <summary>
        /// Gets a value indicating whether the Event is public or not.
        /// </summary>
        public bool IsPublic { get; }
    }
}
