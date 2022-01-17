// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents a projection event key selector.
/// </summary>
public class KeySelector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeySelector"/> class.
    /// </summary>
    /// <param name="type">The <see cref="KeySelectorType"/>.</param>
    public KeySelector(KeySelectorType type)
    {
        Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeySelector"/> class.
    /// </summary>
    /// <param name="type">The <see cref="KeySelectorType"/>.</param>
    /// <param name="expression">The <see cref="KeySelectorExpression"/>.</param>
    public KeySelector(KeySelectorType type, KeySelectorExpression expression)
    {
        Type = type;
        Expression = expression;
    }

    /// <summary>
    /// Gets the <see cref="KeySelectorType" />.
    /// </summary>
    public KeySelectorType Type { get; }

    /// <summary>
    /// Gets the <see cref="KeySelectorExpression" />.
    /// </summary>
    public KeySelectorExpression Expression { get; }
}