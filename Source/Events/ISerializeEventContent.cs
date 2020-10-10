// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Defines a system that is capable of serializing and deserializing event content.
    /// </summary>
    public interface ISerializeEventContent
    {
        /// <summary>
        /// Try to serialize the given object to a string.
        /// </summary>
        /// <param name="content">The content to be serialized.</param>
        /// <param name="jsonString">When the method returns, the serialized json string if the serialization was succesful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the serialization was succesful, otherwise the occurred exception.</param>
        /// <returns>A value indicating whether or not the serialization was succesful.</returns>
        bool TryToSerialize(object content, out string jsonString, out Exception error);

        /// <summary>
        /// Try to deserialize the given string into an object.
        /// </summary>
        /// <param name="source">Json string to be deserialized.</param>
        /// <param name="content">When the method returns, the deserialized object if the serializaton was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the deserialization was successful, otherwise the occurred exception.</param>
        /// <returns>The deserialized object.</returns>
        bool TryToDeserialize(string source, out object content, out Exception error);

        /// <summary>
        /// Try to deserialize the given string into an object.
        /// </summary>
        /// <param name="source">Json string to be deserialized.</param>
        /// <param name="eventType">The <see cref="EventType"/> to deserialize into.</param>
        /// <param name="content">When the method returns, the deserialized object if the serializaton was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the deserialization was successful, otherwise the occurred exception.</param>
        /// <returns>The deserialized object.</returns>
        bool TryToDeserialize(string source, EventType eventType, out object content, out Exception error);
    }
}
