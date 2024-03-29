// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents the concept of an alias for a Projection.
/// </summary>
/// <param name="Value"></param>
public record ProjectionAlias(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="ProjectionAlias"/>.
    /// </summary>
    /// <param name="alias">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="ProjectionAlias"/>.</returns>
    public static implicit operator ProjectionAlias(string alias) => new(alias);
}
