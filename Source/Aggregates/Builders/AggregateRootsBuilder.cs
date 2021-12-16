// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootsBuilder"/>.
/// </summary>
public class AggregateRootsBuilder : ClientArtifactsBuilder<AggregateRootType, AggregateRootId, IUnregisteredAggregateRoots, AggregateRootAttribute>, IAggregateRootsBuilder
{
    /// <inheritdoc />
    public IAggregateRootsBuilder Register<T>()
        where T : class
        => Register(typeof(T));
    
    /// <inheritdoc />
    public IAggregateRootsBuilder Register(Type type)
    {
        Add(type);
        return this;
    }
    
    /// <inheritdoc />
    public IAggregateRootsBuilder RegisterAllFrom(Assembly assembly)
    {
        AddAllFrom(assembly);
        return this;
    }

    /// <summary>
    /// Builds the aggregate roots by registering them with the Runtime.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public IUnregisteredAggregateRoots Build(IClientBuildResults buildResults) => base.Build(buildResults);

    /// <inheritdoc />
    protected override IUnregisteredAggregateRoots CreateUniqueBindings(IClientBuildResults aggregatedBuildResults, IUniqueBindings<AggregateRootType, Type> bindings)
        => new UnregisteredAggregateRoots(bindings);

    /// <inheritdoc />
    protected override bool TryGetIdentifierFromDecorator(Type type, AggregateRootAttribute attribute, out AggregateRootType artifact)
    {
        if (!attribute.Type.HasAlias)
        {
            artifact = new AggregateRootType(attribute.Type.Id, attribute.Type.Generation, type.Name);
            return true;
        }

        artifact = attribute.Type;
        return true;
    }
}
