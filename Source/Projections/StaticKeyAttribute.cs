// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Decorates a projection method with the <see cref="KeySelectorType.Static" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class StaticKeyAttribute : Attribute, IKeySelectorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticKeyAttribute"/> class.
    /// </summary>
    /// <param name="staticKey">The name of the property.</param>
    public StaticKeyAttribute(string staticKey) => StaticKey = staticKey;

    /// <summary>
    /// Gets the static <see cref="Key" />.
    /// </summary>
    public Key StaticKey { get; }

    /// <inheritdoc/>
    public KeySelector KeySelector => KeySelector.Static(StaticKey);
}
