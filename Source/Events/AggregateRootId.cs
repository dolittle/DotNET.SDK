// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents the unique identifier of an aggregate root.
    /// </summary>
    public class AggregateRootId : ArtifactId
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="AggregateRootId"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="ArtifactId"/>.</returns>
        public static implicit operator AggregateRootId(Guid id) => new AggregateRootId { Value = id };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="AggregateRootId"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="AggregateRootId"/>.</returns>
        public static implicit operator AggregateRootId(string id) => new AggregateRootId { Value = Guid.Parse(id) };
    }
}
