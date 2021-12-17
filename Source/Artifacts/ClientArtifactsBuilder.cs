// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents an implementation of <see cref="IArtifactsBuilder{TArtifact,TId,TUniqueBindings}"/>.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
/// <typeparam name="TArtifactAttribute">The <see cref="Type"/> of the <see cref="Attribute"/> used to mark the artifact.</typeparam>
public abstract class ClientArtifactsBuilder<TArtifact, TId, TUniqueBindings, TArtifactAttribute> : ClientUniqueDecoratedBindingsBuilder<TArtifact, Type, TUniqueBindings, TArtifactAttribute>, IArtifactsBuilder<TArtifact, TId, TUniqueBindings>
    where TArtifact : Artifact<TId>, IEquatable<TArtifact>
    where TId : ArtifactId, IEquatable<TId>
    where TUniqueBindings : IUniqueBindings<TArtifact, Type>
    where TArtifactAttribute : Attribute
{
    /// <inheritdoc />
    public void AddAllFrom(Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes)
        {
            if (TryGetDecorator(type, out _))
            {
                Add(type);
            }
        }
    }
}
