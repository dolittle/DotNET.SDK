// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Represents a projection key.
    /// </summary>
    public class Key : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="Key"/>.
        /// </summary>
        /// <param name="expression">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="Key"/>.</returns>
        public static implicit operator Key(string expression) => new Key { Value = expression };
    }
}