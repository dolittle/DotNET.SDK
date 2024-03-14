// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a projection method with the <see cref="KeySelectorType.Property" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeyFromPropertyAttribute : Attribute, IKeySelectorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyFromPropertyAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    public KeyFromPropertyAttribute(string propertyName) => Expression = propertyName;

    /// <summary>
    /// Gets the <see cref="KeySelector" />.
    /// </summary>
    public KeySelectorExpression Expression { get; }

    /// <inheritdoc/>
    public KeySelector KeySelector => KeySelector.Property(Expression);
}
