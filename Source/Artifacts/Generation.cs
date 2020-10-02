// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Artifacts
{
    /// <summary>
    /// Represents a particular stage in the evolution of an artifact, corresponding to a specific form.
    /// </summary>
    public class Generation : ConceptAs<uint>
    {
        /// <summary>
        /// Gets the first generation representation.
        /// </summary>
        public static readonly Generation First = new Generation { Value = 1 };

        /// <summary>
        /// Implicit convertion from Uint to Generation.
        /// </summary>
        /// <param name="value">Value to initialize the <see cref="Generation" /> instance with.</param>
        public static implicit operator Generation(uint value) => new Generation { Value = value };
    }
}
