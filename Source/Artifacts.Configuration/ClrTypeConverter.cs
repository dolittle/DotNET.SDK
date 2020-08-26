// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Dolittle.Artifacts.Configuration
{
    /// <summary>
    /// Represents a <see cref="JsonConverter"/> for dealing with serialization of <see cref="Type"/>.
    /// </summary>
    public class ClrTypeConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(ClrType).IsAssignableFrom(objectType);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var typeName = reader.Value.ToString();
            var clrType = new ClrType { TypeString = typeName };
            return clrType;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = (ClrType)value;
            writer.WriteValue(type.TypeString);
        }
    }
}