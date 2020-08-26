// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Serialization.Json;

namespace Dolittle.Events
{
    /// <summary>
    /// Extends the <see cref="ISerializer"/> with methods related to serialization of events.
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Serialize an <see cref="IEvent"/> to JSON.
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to extend.</param>
        /// <param name="event">The <see cref="IEvent"/> to serialize.</param>
        /// <returns>Camel cased JSON representation of the <see cref="IEvent"/>.</returns>
        public static string EventToJson(this ISerializer serializer, IEvent @event)
            => serializer.ToJson(@event, SerializationOptions.CamelCase);

        /// <summary>
        /// Deserialize from JSON to a given <see cref="IEvent"/> type.
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to extend.</param>
        /// <param name="type">Type of <see cref="IEvent"/> to serialize to.</param>
        /// <param name="json">The JSON representation of the event.</param>
        /// <returns>An instance of the given <see cref="IEvent"/> type.</returns>
        public static IEvent JsonToEvent(this ISerializer serializer, Type type, string json)
            => serializer.FromJson(type, json, SerializationOptions.CamelCase) as IEvent;
    }
}