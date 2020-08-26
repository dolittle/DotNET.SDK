// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Represents the concept of a unique identifier for a stream.
    /// </summary>
    public class FilterId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="FilterId"/>.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> to convert from.</param>
        public static implicit operator FilterId(Guid id) => new FilterId { Value = id };
    }
}