// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.Common;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootsBuilder"/>.
/// </summary>
public class AggregateRootsBuilder : IAggregateRootsBuilder
{
    readonly DecoratedTypeBindingsToModelAdder<AggregateRootAttribute, AggregateRootType, AggregateRootId> _decoratedTypeBindings;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="buildResults"></param>
    public AggregateRootsBuilder(IModelBuilder modelBuilder, IClientBuildResults buildResults)
    {
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<AggregateRootAttribute, AggregateRootType, AggregateRootId>(
            "aggregate root",
            modelBuilder,
            buildResults);
    }

    /// <inheritdoc />
    public IAggregateRootsBuilder Register<T>()
        where T : class
        => Register(typeof(T));
    
    /// <inheritdoc />
    public IAggregateRootsBuilder Register(Type type)
    {
        _decoratedTypeBindings.TryAdd(type, out _);
        return this;
    }
    
    /// <inheritdoc />
    public IAggregateRootsBuilder RegisterAllFrom(Assembly assembly)
    {
        _decoratedTypeBindings.AddFromAssembly(assembly);
        return this;
    }

    /// <summary>
    /// Builds the aggregate roots by registering them with the Runtime.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static IUnregisteredAggregateRoots Build(IApplicationModel applicationModel)
    {
        var bindings = applicationModel.GetTypeBindings<AggregateRootType, AggregateRootId>();
        return new UnregisteredAggregateRoots(new UniqueBindings<AggregateRootType, Type>(bindings.ToDictionary(_ => _.Identifier, _ => _.Type)));
    }
}
