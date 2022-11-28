// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents the base representation of an artifact.
/// </summary>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
public abstract class Artifact<TId> : IIdentifier<TId>, IEquatable<Artifact<TId>>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Artifact{TId}"/> class.
    /// </summary>
    /// <param name="id"><typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.</param>
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="Artifact{TId}"/>.</param>
    protected Artifact(TId id, Generation generation)
    {
        Id = id;
        Generation = generation;
    }
    
    /// <summary>
    /// Gets the <typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.
    /// </summary>
    public TId Id { get; }

    /// <summary>
    /// Gets the <see cref="Generation">Generation</see> of the <see cref="Artifact{TId}"/>.
    /// </summary>
    public Generation Generation { get; }

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
    public bool CanCoexistWith(IIdentifier identifier) => identifier is IIdentifier<TId> typedIdentifier && CanCoexistWith(typedIdentifier);

    /// <inheritdoc />
    public bool Equals(Artifact<TId> other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return EqualityComparer<TId>.Default.Equals(Id, other.Id) && Equals(Generation, other.Generation);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        return obj.GetType() == GetType() && Equals((Artifact<TId>) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (EqualityComparer<TId>.Default.GetHashCode(Id) * 397) ^ Generation.GetHashCode();
        }
    }
}
