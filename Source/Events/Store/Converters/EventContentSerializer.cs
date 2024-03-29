// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Represents an implementation of <see cref="ISerializeEventContent" />.
/// </summary>
public class EventContentSerializer : ISerializeEventContent
{
    readonly IEventTypes _eventTypes;
    readonly Func<JsonSerializerSettings> _jsonSerializerSettingsProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventContentSerializer"/> class.
    /// </summary>
    /// <param name="eventTypes"><see cref="IEventTypes"/> for mapping types and artifacts.</param>
    /// <param name="jsonSerializerSettingsProvider"><see cref="Func{T}"/> that provides <see cref="JsonSerializerSettings"/>.</param>
    public EventContentSerializer(IEventTypes eventTypes, Func<JsonSerializerSettings> jsonSerializerSettingsProvider)
    {
        _eventTypes = eventTypes;
        _jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
    }

    /// <inheritdoc/>
    public bool TrySerialize(object content, [NotNullWhen(true)] out string? json, [NotNullWhen(false)] out Exception? error)
    {
        if (!TrySerializeWithSettings(content, out json, out var serializationError))
        {
            error = new CouldNotSerializeEventContent(content, serializationError);
            return false;
        }

        error = null;
        return true;
    }

    /// <inheritdoc/>
    public bool TryDeserialize(EventType eventType, EventLogSequenceNumber sequenceNumber, string json, [NotNullWhen(true)] out object? content, [NotNullWhen(false)] out Exception? error)
        => TryDeserializeWithSettings(eventType, sequenceNumber, json, out content, out error);

    bool TrySerializeWithSettings(object content, [NotNullWhen(true)] out string? json, [NotNullWhen(false)] out Exception? serializationError)
    {
        var exceptionCatcher = new JsonSerializerExceptionCatcher();
        var serializerSettings = _jsonSerializerSettingsProvider();
        serializerSettings.Error = exceptionCatcher.OnError;
        serializerSettings.Formatting = Formatting.None;

        json = JsonConvert.SerializeObject(content, serializerSettings);

        if (exceptionCatcher.Failed)
        {
            serializationError = new CouldNotSerializeEventContent(content, exceptionCatcher.Error);
            return false;
        }
        else
        {
            serializationError = null;
            return true;
        }
    }

    bool TryDeserializeWithSettings(EventType eventType, EventLogSequenceNumber sequenceNumber, string json, [NotNullWhen(true)] out object? content,
        [NotNullWhen(false)] out Exception? deserializationError)
    {
        var exceptionCatcher = new JsonSerializerExceptionCatcher();
        var serializerSettings = _jsonSerializerSettingsProvider();
        serializerSettings.Error = exceptionCatcher.OnError;

        if (_eventTypes.HasTypeFor(eventType))
        {
            var type = _eventTypes.GetTypeFor(eventType);
            content = JsonConvert.DeserializeObject(json, type, serializerSettings);

            deserializationError = exceptionCatcher.Failed
                ? new CouldNotDeserializeEventContent(eventType, sequenceNumber, json, exceptionCatcher.Error!, type)
                : null;
        }
        else
        {
            content = JsonConvert.DeserializeObject(json, serializerSettings);

            deserializationError = exceptionCatcher.Failed
                ? new CouldNotDeserializeEventContent(eventType, sequenceNumber, json, exceptionCatcher.Error!)
                : null;
        }

        return !exceptionCatcher.Failed;
    }
}
