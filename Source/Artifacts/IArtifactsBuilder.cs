// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Defines a builder for that can build <see cref="Type"/> to <typeparamref name="TArtifact"/> associations.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
public interface IArtifactsBuilder<TArtifact, TId>
    where TArtifact : Artifact<TId>
    where TId : ArtifactId
{
    /// <summary>
    /// Register an relationship between an <see cref="Artifact{TId}"/> and a <see cref="Type"/>.
    /// </summary>
    /// <param name="type"><see cref="Type"/> associated with the artifact.</param>
    /// <param name="artifact"><see cref="Artifact{TId}"/> to register.</param>
    void Associate(Type type, TArtifact artifact);
    
    /// <summary>
    /// Register a <see cref="Type"/> with the artifact given by the attribute.
    /// </summary>
    /// <param name="type"><see cref="Type"/> associated with the artifact.</param>
    void Register(Type type);
    
    /// <summary>
    /// Register a <see cref="Type"/> with the artifact given by the attribute.
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/> to register all artifact classes from.</param>
    void RegisterAllFrom(Assembly assembly);

    /// <summary>
    /// Builds the <see cref="Type"/> to <typeparamref name="TArtifact"/> associations.
    /// </summary>
    /// <returns>The <see cref="Type"/> to <typeparamref name="TArtifact"/> associations.</returns>
    IDictionary<Type, TArtifact> Build();
}
