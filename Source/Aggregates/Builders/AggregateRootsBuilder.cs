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
public class AggregateRootsBuilder : ClientArtifactsBuilder<AggregateRootType, AggregateRootId, AggregateRootAttribute>, IAggregateRootsBuilder
{
    /// <summary>
    /// Initializes an instance of the <see cref="AggregateRootsBuilder"/> class.
    /// </summary>
    /// <param name="clientBuildResults">The <see cref="IClientBuildResults"/>.</param>
    public AggregateRootsBuilder(IClientBuildResults clientBuildResults)
        : base(clientBuildResults) 
    {
    }

    /// <inheritdoc />
    public IAggregateRootsBuilder Register<T>()
        where T : class
        => Register(typeof(T));
    
    /// <inheritdoc />
    public new IAggregateRootsBuilder Register(Type type)
    {
        base.Register(type);
        return this;
    }
    
    /// <inheritdoc />
    public new IAggregateRootsBuilder RegisterAllFrom(Assembly assembly)
    {
        base.RegisterAllFrom(assembly);
        return this;
    }

    /// <summary>
    /// Builds the aggregate roots by registering them with the Runtime.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public new IUnregisteredAggregateRoots Build()
        => new UnregisteredAggregateRoots(base.Build());
    
    /// <inheritdoc />
    protected override bool TryGetArtifactFromAttribute(Type type, AggregateRootAttribute attribute, out AggregateRootType artifact)
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
