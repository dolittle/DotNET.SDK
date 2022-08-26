// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents the base representation of an artifact.
/// </summary>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
public abstract class Artifact<TId> : Identifier<TId, Generation>, IEquatable<Artifact<TId>>
    where TId : ArtifactId
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Artifact{TId}"/> class.
    /// </summary>
    /// <param name="id"><typeparamref name="TId">Id</typeparamref> of the <see cref="Artifact{TId}"/>.</param>
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="Artifact{TId}"/>.</param>
    /// <param name="alias">The artifact alias.</param>
    protected Artifact(TId id, Generation? generation = null, IdentifierAlias? alias = null)
        : base(id, generation ?? Generation.First, alias)
    {
        Tag = GetType().Name;
        Generation = generation ?? Generation.First;
    }

    /// <summary>
    /// Gets the <see cref="Generation">Generation</see> of the <see cref="Artifact{TId}"/>.
    /// </summary>
    public Generation Generation { get; }

    /// <inheritdoc />
    public override bool CanCoexistWith(IIdentifier<ConceptAs<Guid>> identifier)
    {
        if (identifier.GetType() != GetType())
        {
            return false;
        }
        var artifact = identifier as Artifact<TId>;
        return Id.Equals(artifact?.Id) && !Generation.Equals(artifact?.Generation);
    }

    /// <inheritdoc />
    public bool Equals(Artifact<TId>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return base.Equals(other);
    }
}
