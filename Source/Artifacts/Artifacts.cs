// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents an implementation of <see cref="IArtifacts{TArtifact,TId}" />.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
public abstract class Artifacts<TArtifact, TId> : IArtifacts<TArtifact, TId>
    where TArtifact : Artifact<TId>
    where TId : ArtifactId
{
    readonly IDictionary<TArtifact, Type> _artifactToTypeMap;
    readonly IDictionary<Type, TArtifact> _typeToArtifactMap;

    /// <summary>
    /// Initializes an instance of the <see cref="Artifacts{TArtifact,TId}"/> class.
    /// </summary>
    /// <param name="associations">The artifact associations.</param>
    protected Artifacts(IDictionary<Type, TArtifact> associations)
    {
        _typeToArtifactMap = associations;
        _artifactToTypeMap = associations.ToDictionary(_ => _.Value, _ => _.Key);
    }

    /// <inheritdoc/>
    public IEnumerable<TArtifact> All => _artifactToTypeMap.Keys;
    
    /// <inheritdoc/>
    public IEnumerable<Type> Types => _typeToArtifactMap.Keys;

    /// <inheritdoc />
    public TArtifact GetFor<T>()
        where T : class
        => GetFor(typeof(T));

    /// <inheritdoc />
    public TArtifact GetFor(Type type)
    {
        if (!_typeToArtifactMap.TryGetValue(type, out var artifact))
        {
            throw CreateNoArtifactAssociatedWithType(type);
        }
        return artifact;
    }

    /// <inheritdoc />
    public Type GetTypeFor(TArtifact artifact)
    {
        if (!_artifactToTypeMap.TryGetValue(artifact, out var type))
        {
            throw CreateNoTypeAssociatedWithArtifact(artifact);
        }
        return type;
    }

    /// <inheritdoc />
    public bool HasFor<T>()
        where T : class
        => HasFor(typeof(T));

    /// <inheritdoc />
    public bool HasFor(Type type) => _typeToArtifactMap.ContainsKey(type);

    /// <inheritdoc />
    public bool HasTypeFor(TArtifact artifact) => _artifactToTypeMap.ContainsKey(artifact);

    /// <summary>
    /// Create <see cref="Exception"/> to throw when no <typeparamref name="TArtifact"/> is associated with the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that has no association.</param>
    /// <returns>The <see cref="Exception"/> to throw.</returns>
    protected abstract Exception CreateNoArtifactAssociatedWithType(Type type);

    /// <summary>
    /// Create <see cref="Exception"/> to throw when no <see cref="Type"/> is associated with the given <typeparamref name="TArtifact"/>.
    /// </summary>
    /// <param name="artifact">The <typeparamref name="TArtifact"/> that has no association.</param>
    /// <returns>The <see cref="Exception"/> to throw.</returns>
    protected abstract Exception CreateNoTypeAssociatedWithArtifact(TArtifact artifact);
}
