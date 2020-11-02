// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store.Converters
{
    /// <summary>
    /// Exception that gets thrown when an the committed content of an event could not be deserialized.
    /// </summary>
    public class CouldNotDeserializeEventContent : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotDeserializeEventContent"/> class.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType"/> of the Event.</param>
        /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber"/> of the Event.</param>
        /// <param name="json">The JSON content of the event.</param>
        /// <param name="innerException">The exception thrown by the deserializer.</param>
        /// <param name="type">The optional Type of the event.</param>
        public CouldNotDeserializeEventContent(
            EventType eventType,
            EventLogSequenceNumber sequenceNumber,
            string json,
            Exception innerException,
            Type type = default)
            : base(CreateExceptionMessage(eventType, sequenceNumber, json, type), innerException)
        {
        }

        static string CreateExceptionMessage(EventType eventType, EventLogSequenceNumber sequenceNumber, string json, Type type)
        {
            if (type == default)
                return $"Could not deserialize event with sequence number {sequenceNumber} and event type {eventType}. Content is {json}";
            else
                return $"Could not deserialize event with sequence number {sequenceNumber} and event type {eventType} to {type}. Content is {json}";
        }
    }
}
