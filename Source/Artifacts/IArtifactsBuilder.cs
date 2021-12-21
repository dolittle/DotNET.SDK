// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Defines a builder for that can build <see cref="Type"/> to <typeparamref name="TArtifact"/> associations.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
public interface IArtifactsBuilder<TArtifact, TId> : ICanBuildUniqueDecoratedBindings<TArtifact, Type>
    where TArtifact : Artifact<TId>, IEquatable<TArtifact>
    where TId : ArtifactId, IEquatable<TId>
{
    /// <summary>
    /// Adds all bindings discovered in the <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/> to add all bindings from.</param>
    void AddAllFrom(Assembly assembly);
}
