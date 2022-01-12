// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Defines a system that knows about <see cref="Artifact{TId}"/> associated to a <see cref="Type"/>.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
public interface IArtifacts<TArtifact, TId>
    where TArtifact : Artifact<TId>, IEquatable<TArtifact>
    where TId : ArtifactId
{
    /// <summary>
    /// Gets all artifacts.
    /// </summary>
    IEnumerable<TArtifact> All { get; }
        
    /// <summary>
    /// Gets all the <see cref="Type"/> types.
    /// </summary>
    IEnumerable<Type> Types { get; }
    
    /// <summary>
    /// Check if there an <see cref="Artifact{TId}"/> binding for a <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the artifact.</typeparam>
    /// <returns><see cref="bool"/>.</returns>
    bool HasFor<T>()
        where T : class;

    /// <summary>
    /// Get an <see cref="Artifact{TId}"/> bound to the given <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the artifact.</typeparam>
    /// <returns><see cref="Artifact{TId}"/>.</returns>
    TArtifact GetFor<T>()
        where T : class;

    /// <summary>
    /// Check if there is an <typeparamref name="TArtifact"/> associated with a <see cref="Type" />.
    /// </summary>
    /// <param name="artifact">The <typeparamref name="TArtifact"/>.</param>
    /// <returns><see cref="bool"/>.</returns>
    bool HasTypeFor(TArtifact artifact);

    /// <summary>
    /// Gets the <see cref="Type"/> bound t a specific <see cref="Artifact{TId}"/>.
    /// </summary>
    /// <param name="artifact"><see cref="Artifact{TId}"/> to get for.</param>
    /// <returns><see cref="Type"/>.</returns>
    Type GetTypeFor(TArtifact artifact);
}
