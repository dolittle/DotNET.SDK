// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents the unique identifier of an <see cref="EventType" />.
/// </summary>
public record EventTypeId(Guid Value) : ArtifactId(Value)
{
    /// <summary>
    /// Implicitly converts from a <see cref="Guid"/> to an <see cref="EventTypeId"/>.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> representation.</param>
    /// <returns>The converted <see cref="ArtifactId"/>.</returns>
    public static implicit operator EventTypeId(Guid id) => new(id);

    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="EventTypeId"/>.
    /// </summary>
    /// <param name="id">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="EventTypeId"/>.</returns>
    public static implicit operator EventTypeId(string id) => new(Guid.Parse(id));
}
