// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents the concept of a unique identifier for a stream.
    /// </summary>
    public class StreamId : ConceptAs<Guid>
    {
        /// <summary>
        /// Gets the unspecified partition id.
        /// </summary>
        public static StreamId EventLog => new StreamId { Value = Guid.Empty };

        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="StreamId"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="StreamId"/>.</returns>
        public static implicit operator StreamId(Guid id) => new StreamId { Value = id };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="StreamId"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="StreamId"/>.</returns>
        public static implicit operator StreamId(string id) => new StreamId { Value = Guid.Parse(id) };
    }
}
