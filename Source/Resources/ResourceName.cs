// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Resources;

/// <summary>
/// Represents the name of an <see cref="IResource" />.
/// </summary>
public record ResourceName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly convert from a <see cref="string"/> to a <see cref="ResourceName"/>.
    /// </summary>
    /// <param name="name">ResourceName as <see cref="string"/>.</param>
    public static implicit operator ResourceName(string name) => new(name);
}