// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Dolittle.SDK.Events.Store.Converters
{
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
        public EventContentSerializer(IEventTypes eventTypes, Func<JsonSerializerSettings> jsonSerializerSettingsProvider = null)
        {
            _eventTypes = eventTypes;
            _jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
            _jsonSerializerSettingsProvider ??= () => new JsonSerializerSettings();
        }

        /// <inheritdoc/>
        public bool TrySerialize(object content, out string json, out Exception error)
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
        public bool TryDeserialize(EventType eventType, EventLogSequenceNumber sequenceNumber, string json, out object content, out Exception error)
            => TryDeserializeWithSettings(eventType, sequenceNumber, json, out content, out error);

        bool TrySerializeWithSettings(object content, out string json, out Exception serializationError)
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

        bool TryDeserializeWithSettings(EventType eventType, EventLogSequenceNumber sequenceNumber, string json, out object content, out Exception deserializationError)
        {
            var exceptionCatcher = new JsonSerializerExceptionCatcher();
            var serializerSettings = _jsonSerializerSettingsProvider();
            serializerSettings.Error = exceptionCatcher.OnError;

            if (_eventTypes.HasTypeFor(eventType))
            {
                var type = _eventTypes.GetTypeFor(eventType);
                content = JsonConvert.DeserializeObject(json, type, serializerSettings);

                if (exceptionCatcher.Failed)
                    deserializationError = new CouldNotDeserializeEventContent(eventType, sequenceNumber, json, exceptionCatcher.Error, type);
                else
                    deserializationError = null;
            }
            else
            {
                content = JsonConvert.DeserializeObject(json, serializerSettings);

                if (exceptionCatcher.Failed)
                    deserializationError = new CouldNotDeserializeEventContent(eventType, sequenceNumber, json, exceptionCatcher.Error);
                else
                    deserializationError = null;
            }

            return !exceptionCatcher.Failed;
        }
    }
}
