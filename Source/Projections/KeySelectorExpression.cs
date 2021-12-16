// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Represents the key selector expression.
    /// </summary>
    public record KeySelectorExpression(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="KeySelectorExpression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="KeySelectorExpression"/>.</returns>
        public static implicit operator KeySelectorExpression(string expression) => new(expression);
    }
}
