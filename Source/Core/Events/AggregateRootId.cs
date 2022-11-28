// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents the unique identifier of an aggregate root.
/// </summary>
public record AggregateRootId(Guid Value) : ArtifactId(Value)
{
    /// <summary>
    /// Implicitly converts from a <see cref="Guid"/> to an <see cref="AggregateRootId"/>.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> representation.</param>
    /// <returns>The converted <see cref="ArtifactId"/>.</returns>
    public static implicit operator AggregateRootId(Guid id) => new(id);

    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="AggregateRootId"/>.
    /// </summary>
    /// <param name="id">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="AggregateRootId"/>.</returns>
    public static implicit operator AggregateRootId(string id) => new(Guid.Parse(id));
}
