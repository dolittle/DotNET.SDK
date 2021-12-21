// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents an implementation of <see cref="IArtifactsBuilder{TArtifact,TId}"/>.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
/// <typeparam name="TArtifactAttribute">The <see cref="Type"/> of the <see cref="Attribute"/> used to mark the artifact.</typeparam>
public class ClientArtifactsBuilder<TArtifact, TId, TArtifactAttribute> : ClientUniqueDecoratedBindingsBuilder<TArtifact, Type, TArtifactAttribute>, IArtifactsBuilder<TArtifact, TId>
    where TArtifact : Artifact<TId>, IEquatable<TArtifact>
    where TId : ArtifactId, IEquatable<TId>
    where TArtifactAttribute : Attribute, IUniqueBindingDecorator<TArtifact>
{
    /// <summary>
    /// Initializes an instance of the <see cref="ClientArtifactsBuilder{TArtifact,TId,TArtifactAttribute}"/> class.
    /// </summary>
    /// <param name="identifierLabel">The label of the identifier. Used for <see cref="IClientBuildResults"/>.</param>
    /// <param name="valueLabel">The label of the value. Used for <see cref="IClientBuildResults"/>.</param>
    public ClientArtifactsBuilder(string identifierLabel = nameof(TArtifact), string valueLabel = nameof(Type))
        : base(identifierLabel, valueLabel)
    {
    }
    
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
