// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents the context in which an event occurred in.
    /// </summary>
    public class EventContext : Value<EventContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventContext"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber">sequence number</see> that uniquely identifies the event in the event log which it was committed.</param>
        /// <param name="eventType">The <see cref="EventType"/> of the event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> that the event was committed to.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset"/> when the event was committed to the <see cref="IEventStore"/>.</param>
        /// <param name="committedExecutionContext">The <see cref="ExecutionContext"/> in which the event was committed to the <see cref="IEventStore"/>.</param>
        /// <param name="currentExecutionContext">The <see cref="ExecutionContext"/> in which the event is currently being processed.</param>
        public EventContext(
            EventLogSequenceNumber sequenceNumber,
            EventType eventType,
            EventSourceId eventSourceId,
            DateTimeOffset occurred,
            ExecutionContext committedExecutionContext,
            ExecutionContext currentExecutionContext)
        {
            SequenceNumber = sequenceNumber;
            Type = eventType;
            EventSourceId = eventSourceId;
            Occurred = occurred;
            CommittedExecutionContext = committedExecutionContext;
            CurrentExecutionContext = currentExecutionContext;
        }

        /// <summary>
        /// Gets the <see cref="EventLogSequenceNumber">sequence number</see> that uniquely identifies the event in the event log which it was committed.
        /// </summary>
        public EventLogSequenceNumber SequenceNumber { get; }

        /// <summary>
        /// Gets the <see cref="EventType" /> of the event.
        /// </summary>
        public EventType Type { get; }

        /// <summary>
        /// Gets the <see cref="EventSourceId"/> that the event was committed to.
        /// </summary>
        public EventSourceId EventSourceId { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset"/> when the event was committed to the <see cref="IEventStore"/>.
        /// </summary>
        public DateTimeOffset Occurred { get; }

        /// <summary>
        /// Gets the <see cref="ExecutionContext"/> in which the event was committed to the <see cref="IEventStore"/>.
        /// </summary>
        public ExecutionContext CommittedExecutionContext { get; }

        /// <summary>
        /// Gets the <see cref="ExecutionContext"/> in which the event was committed to the <see cref="IEventStore"/>.
        /// </summary>
        public ExecutionContext CurrentExecutionContext { get; }
    }
}