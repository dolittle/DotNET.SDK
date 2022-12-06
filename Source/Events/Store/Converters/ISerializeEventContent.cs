// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Defines a system that is capable of serializing and deserializing event content.
/// </summary>
public interface ISerializeEventContent
{
    /// <summary>
    /// Try to serialize the given object to JSON.
    /// </summary>
    /// <param name="content">The content to be serialized.</param>
    /// <param name="json">When the method returns, the serialized JSON if the serialization was succesful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the serialization was succesful, otherwise the occurred exception.</param>
    /// <returns>A value indicating whether or not the serialization was succesful.</returns>
    bool TrySerialize(object content, [NotNullWhen(true)] out string? json, [NotNullWhen(false)] out Exception? error);

    /// <summary>
    /// Try to deserialize the given JSON into an object with a given type.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType"/> of the Event.</param>
    /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber"/> of the Event.</param>
    /// <param name="json">JSON to be deserialized.</param>
    /// <param name="content">When the method returns, the deserialized object if the serializaton was successful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the deserialization was successful, otherwise the occurred exception.</param>
    /// <returns>The deserialized object, of type <see cref="object"/> if the event type is not assoicated with a type, otherwise the associated type.</returns>
    bool TryDeserialize(EventType eventType, EventLogSequenceNumber sequenceNumber, string json, [NotNullWhen(true)] out object? content, [NotNullWhen(false)] out Exception? error);
}
