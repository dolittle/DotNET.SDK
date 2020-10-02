// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the concept of a unique identifier for an event filter.
    /// </summary>
    public class FilterId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="FilterId"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="FilterId"/>.</returns>
        public static implicit operator FilterId(Guid id) => new FilterId { Value = id };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="FilterId"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="FilterId"/>.</returns>
        public static implicit operator FilterId(string id) => new FilterId { Value = Guid.Parse(id) };
    }
}