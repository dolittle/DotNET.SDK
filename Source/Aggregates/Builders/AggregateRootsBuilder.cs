// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootsBuilder"/>.
/// </summary>
public class AggregateRootsBuilder : IAggregateRootsBuilder
{
    readonly ClientArtifactsBuilder<AggregateRootType, AggregateRootId, AggregateRootAttribute> _artifactsBuilder = new();
    /// <inheritdoc />
    public IAggregateRootsBuilder Register<T>()
        where T : class
        => Register(typeof(T));
    
    /// <inheritdoc />
    public IAggregateRootsBuilder Register(Type type)
    {
        _artifactsBuilder.Add(type);
        return this;
    }
    
    /// <inheritdoc />
    public IAggregateRootsBuilder RegisterAllFrom(Assembly assembly)
    {
        _artifactsBuilder.AddAllFrom(assembly);
        return this;
    }

    /// <summary>
    /// Builds the aggregate roots by registering them with the Runtime.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public IUnregisteredAggregateRoots Build(IClientBuildResults buildResults) => new UnregisteredAggregateRoots(_artifactsBuilder.Build(buildResults));
}
