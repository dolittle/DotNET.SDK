// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents the name of a projection property.
/// </summary>
/// <param name="Value">The collection name.</param>
public record PropertyName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="PropertyName"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="PropertyName"/>.</returns>
    public static implicit operator PropertyName(string value) => value != default ? new PropertyName(value) : null;
    
    /// <summary>
    /// Implicitly converts the <see cref="PropertyName"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="field">The <see cref="PropertyName"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(PropertyName field) => field?.Value;
}
