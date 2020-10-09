// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when an Event couldn't be deserialized.
    /// </summary>s
    public class CouldNotDeserializeEvent : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotDeserializeEvent"/> class.
        /// </summary>
        /// <param name="id">The <see cref="EventTypeId"/> of the Events type.</param>
        /// <param name="eventType">The Type of the event.</param>
        /// <param name="content">The content of the event.</param>
        /// <param name="sequenceNumber">The Events position in the event log.</param>
        /// <param name="innerException">The exception thrown by the deserializer.</param>
        public CouldNotDeserializeEvent(
            EventTypeId id,
            Type eventType,
            string content,
            ulong sequenceNumber,
            Exception innerException)
            : base($"Couldn't deserialize EventType '{id}' to event '{eventType}' with EventLogSequenceNumber '{sequenceNumber}' and content '{content}'", innerException)
        {
        }
    }
}
