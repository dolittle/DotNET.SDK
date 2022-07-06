// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents an implementation of <see cref="IArtifacts{TArtifact,TId}" />.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
public abstract class Artifacts<TArtifact, TId> : UniqueBindings<TArtifact, Type>, IArtifacts<TArtifact, TId>
    where TArtifact : Artifact<TId>
    where TId : ArtifactId
{
    /// <summary>
    /// Initializes an instance of the <see cref="Artifacts{TArtifact,TId}"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="IUniqueBindings{TIdentifier,TValue}"/>.</param>
    protected Artifacts(IUniqueBindings<TArtifact, Type> bindings)
        : base(bindings)
    {
    }

    /// <summary>
    /// Initializes an instance of the <see cref="Artifacts{TArtifact,TId}"/> class.
    /// </summary>
    protected Artifacts()
    {
    }

    /// <inheritdoc/>
    public IEnumerable<TArtifact> All => Keys;
    
    /// <inheritdoc/>
    public IEnumerable<Type> Types => Values;

    /// <inheritdoc />
    public TArtifact GetFor<T>()
        where T : class
        => GetFor(typeof(T));

    /// <inheritdoc />
    public Type GetTypeFor(TArtifact artifact) => base.GetFor(artifact);

    /// <inheritdoc />
    public bool HasFor<T>()
        where T : class
        => HasFor(typeof(T));

    /// <inheritdoc />
    public bool HasTypeFor(TArtifact artifact) => base.HasFor(artifact);
}
