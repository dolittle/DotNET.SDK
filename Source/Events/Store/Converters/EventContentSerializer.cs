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
        readonly JsonSerializerSettings _jsonSerializerSettings;
        readonly JsonSerializerExceptionCatcher _exceptionCatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventContentSerializer"/> class.
        /// </summary>
        /// <param name="eventTypes"><see cref="IEventTypes"/> for mapping types and artifacts.</param>
        /// <param name="jsonSerializerSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public EventContentSerializer(IEventTypes eventTypes, JsonSerializerSettings jsonSerializerSettings = null)
        {
            _eventTypes = eventTypes;
            _jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();

            _exceptionCatcher = new JsonSerializerExceptionCatcher();
            _jsonSerializerSettings.Error = _exceptionCatcher.OnError;
            _jsonSerializerSettings.Formatting = Formatting.None;
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
            json = JsonConvert.SerializeObject(content, _jsonSerializerSettings);

            if (_exceptionCatcher.Failed)
            {
                serializationError = new CouldNotSerializeEventContent(content, _exceptionCatcher.Error);
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
            if (_eventTypes.HasTypeFor(eventType))
            {
                var type = _eventTypes.GetTypeFor(eventType);
                content = JsonConvert.DeserializeObject(json, type, _jsonSerializerSettings);

                if (_exceptionCatcher.Failed)
                    deserializationError = new CouldNotDeserializeEventContent(eventType, sequenceNumber, json, _exceptionCatcher.Error, type);
                else
                    deserializationError = null;
            }
            else
            {
                content = JsonConvert.DeserializeObject(json, _jsonSerializerSettings);

                if (_exceptionCatcher.Failed)
                    deserializationError = new CouldNotDeserializeEventContent(eventType, sequenceNumber, json, _exceptionCatcher.Error);
                else
                    deserializationError = null;
            }

            return !_exceptionCatcher.Failed;
        }
    }
}
