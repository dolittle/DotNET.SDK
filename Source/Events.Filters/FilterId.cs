// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the concept of a unique identifier for an event filter.
    /// </summary>
    public record FilterId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="FilterId"/>.
        /// </summary>
        /// <param name="filterId">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="FilterId"/>.</returns>
        public static implicit operator FilterId(Guid filterId) => new(filterId);

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="FilterId"/>.
        /// </summary>
        /// <param name="filterId">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="FilterId"/>.</returns>
        public static implicit operator FilterId(string filterId) => new(Guid.Parse(filterId));
    }
}
