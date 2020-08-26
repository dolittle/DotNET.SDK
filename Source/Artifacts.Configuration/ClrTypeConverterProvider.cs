// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Serialization.Json;
using Newtonsoft.Json;

namespace Dolittle.Artifacts.Configuration
{
    /// <summary>
    /// Represents a class that can provide a <see cref="ClrTypeConverter"/>.
    /// </summary>
    public class ClrTypeConverterProvider : ICanProvideConverters
    {
        /// <inheritdoc/>
        public IEnumerable<JsonConverter> Provide()
        {
            return new JsonConverter[] { new ClrTypeConverter() };
        }
    }
}