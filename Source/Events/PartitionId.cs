// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents the concept of a unique identifier for a partition.
    /// </summary>
    public class PartitionId : ConceptAs<Guid>
    {
        /// <summary>
        /// Gets the unspecified partition id.
        /// </summary>
        public static PartitionId Unspecified => new PartitionId { Value = Guid.Empty };

        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="PartitionId"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="PartitionId"/>.</returns>
        public static implicit operator PartitionId(Guid id) => new PartitionId { Value = id };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="PartitionId"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="PartitionId"/>.</returns>
        public static implicit operator PartitionId(string id) => new PartitionId { Value = Guid.Parse(id) };
    }
}
