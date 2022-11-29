// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootsBuilder"/>.
/// </summary>
public class AggregateRootsBuilder : IAggregateRootsBuilder
{
    readonly DecoratedTypeBindingsToModelAdder<AggregateRootAttribute, AggregateRootType, AggregateRootId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootsBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
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
    public static IUnregisteredAggregateRoots Build(IModel model, IClientBuildResults buildResults)
    {
        var bindings = model.GetTypeBindings<AggregateRootType, AggregateRootId>().ToArray();
        foreach (var binding in bindings)
        {
            buildResults.AddInformation(binding.Identifier, $"Successfully bound to type {binding.Type}");            
        }
        return new UnregisteredAggregateRoots(new UniqueBindings<AggregateRootType, Type>(bindings.ToDictionary(_ => _.Identifier, _ => _.Type)));
    }
}
