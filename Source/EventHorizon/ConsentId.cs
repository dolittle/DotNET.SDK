// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents the concept of a unique identifier for a stream.
    /// </summary>
    public class ConsentId : ConceptAs<Guid>
    {
        /// <summary>
        /// Gets the consent id used if it is not set.
        /// </summary>
        public static ConsentId NotSet => new ConsentId { Value = Guid.Empty };

        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="ConsentId"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="ConsentId"/>.</returns>
        public static implicit operator ConsentId(Guid id) => new ConsentId { Value = id };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="ConsentId"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="ConsentId"/>.</returns>
        public static implicit operator ConsentId(string id) => new ConsentId { Value = Guid.Parse(id) };
    }
}
