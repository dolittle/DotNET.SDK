// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents the base representation of an artifact.
/// </summary>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
/// <param name="Id"><typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.</param>
/// <param name="Generation"><see cref="Generation">Generation</see> of the <see cref="Artifact{TId}"/>.</param>
public abstract record Artifact<TId>(TId Id, Generation Generation)
    where TId : ArtifactId
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Artifact{TId}"/> class.
    /// </summary>
    /// <param name="id"><typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.</param>
    protected Artifact(TId id)
        : this(id, Generation.First)
    {
    }
}
