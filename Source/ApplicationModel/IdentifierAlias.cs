// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Represents the alias for an 
/// </summary>
/// <param name="Value">The alias string value.</param>
public record IdentifierAlias(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="IdentifierAlias"/>.
    /// </summary>
    /// <param name="alias">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="IdentifierAlias"/>.</returns>
    public static implicit operator IdentifierAlias(string alias) => new(alias);

    /// <summary>
    /// Gets a value indicating whether there is an alias or not.
    /// </summary>
    public bool Exists => string.IsNullOrEmpty(Value);
}
