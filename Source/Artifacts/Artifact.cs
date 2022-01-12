// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents the base representation of an artifact.
/// </summary>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
/// <param name="Id"><typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.</param>
/// <param name="Generation"><see cref="Generation">Generation</see> of the <see cref="Artifact{TId}"/>.</param>
public abstract record Artifact<TId>(TId Id, Generation Generation) : IIdentifier<TId>
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

    /// <inheritdoc />
    Guid IIdentifier.Id => Id.Value;

    /// <inheritdoc />
    public bool CanCoexistWith(IIdentifier<ConceptAs<Guid>> identifier)
    {
        if (identifier.GetType() != GetType())
        {
            return false;
        }
        var artifact = identifier as Artifact<TId>;
        return Id.Equals(artifact?.Id) && !Generation.Equals(artifact?.Generation);
    }

    /// <inheritdoc />
    public bool CanCoexistWith(IIdentifier identifier) => identifier is IIdentifier<TId> typedIdentifier && CanCoexistWith((IIdentifier<ConceptAs<Guid>>)typedIdentifier);
}
