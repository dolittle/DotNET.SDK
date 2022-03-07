// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents the concept of a unique identifier for a projection.
/// </summary>
public record ProjectionId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Implicitly converts from a <see cref="Guid"/> to an <see cref="ProjectionId"/>.
    /// </summary>
    /// <param name="projection">The <see cref="Guid"/> representation.</param>
    /// <returns>The converted <see cref="ProjectionId"/>.</returns>
    public static implicit operator ProjectionId(Guid projection) => new(projection);

    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="ProjectionId"/>.
    /// </summary>
    /// <param name="projection">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="ProjectionId"/>.</returns>
    public static implicit operator ProjectionId(string projection) => new(Guid.Parse(projection));
}