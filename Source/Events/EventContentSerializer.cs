// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="ISerializeEventContent" />.
    /// </summary>
    public class EventContentSerializer : ISerializeEventContent
    {
        readonly IEventTypes _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventContentSerializer"/> class.
        /// </summary>
        /// <param name="eventTypes"><see cref="IEventTypes"/> for mapping types and artifacts.</param>
        public EventContentSerializer(IEventTypes eventTypes)
        {
            _eventTypes = eventTypes;
        }

        /// <inheritdoc/>
        public bool TryToSerialize(object content, out string jsonString, out Exception error)
        {
            if (!TryToSerializeWithSettings(content, out jsonString, out var serializationError))
            {
                error = new CouldNotSerializeEventContent(content, serializationError);
                return false;
            }

            error = null;
            return true;
        }

        /// <inheritdoc/>
        public bool TryToDeserialize(string source, out object content, out Exception error)
            => TryToDeserializeWithSettings(source, out content, out error);

        /// <inheritdoc/>
        public bool TryToDeserialize(string source, EventType eventType, out object content, out Exception error)
            => TryToDeserializeWithSettings(source, out content, out error, eventType);

        bool TryToSerializeWithSettings(object content, out string jsonString, out Exception serializationError)
        {
            var serializationFailed = false;
            serializationError = null;

            // have to use an extra var for errors because you can't use 'out' params in lambda expresions
            Exception tempError = null;
            var serializerSettings = new JsonSerializerSettings
            {
                Error = (_, args) =>
                {
                    serializationFailed = true;
                    tempError = args.ErrorContext.Error;
                },
                Formatting = Formatting.None,
            };

            jsonString = JsonConvert.SerializeObject(content, serializerSettings);
            serializationError = tempError;

            return !serializationFailed;
        }

        bool TryToDeserializeWithSettings(string source, out object content, out Exception deserializationError, EventType eventType = default)
        {
            var deserializationFailed = false;
            deserializationError = null;

            // have to use an extra var for errors because you can't use 'out' params in lambda expresions
            Exception tempError = null;
            var serializerSettings = new JsonSerializerSettings
            {
                Error = (_, args) =>
                {
                    deserializationFailed = true;
                    tempError = args.ErrorContext.Error;
                },
                Formatting = Formatting.None,
            };

            if (_eventTypes.HasTypeFor(eventType))
            {
                content = JsonConvert.DeserializeObject(source, _eventTypes.GetTypeFor(eventType), serializerSettings);
            }
            else
            {
                content = JsonConvert.DeserializeObject(source, serializerSettings);
            }

            deserializationError = tempError;

            return !deserializationFailed;
        }
    }
}
