// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Defines a builder for that can build <see cref="Type"/> to <typeparamref name="TArtifact"/> associations.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
public interface IArtifactsBuilder<in TArtifact, TId, out TUniqueBindings> : ICanBuildUniqueDecoratedBindings<TArtifact, TUniqueBindings>
    where TArtifact : Artifact<TId>, IEquatable<TArtifact>
    where TId : ArtifactId, IEquatable<TId>
    where TUniqueBindings : IUniqueBindings<TArtifact, Type>
{
}
