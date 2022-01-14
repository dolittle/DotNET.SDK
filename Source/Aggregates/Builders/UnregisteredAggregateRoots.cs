// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Common;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredAggregateRoots"/> and <see cref="AggregateRootTypes"/>.
/// </summary>
public class UnregisteredAggregateRoots : AggregateRootTypes, IUnregisteredAggregateRoots
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredAggregateRoots"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="IUniqueBindings{TIdentifier,TValue}"/> binding <see cref="AggregateRootType"/> to <see cref="Type"/>.</param>
    public UnregisteredAggregateRoots(IUniqueBindings<AggregateRootType, Type> bindings)
        : base(bindings)
    {
        AddTenantScopedServices = AddToContainer;
    }

    /// <inheritdoc />
    public ConfigureTenantServices AddTenantScopedServices { get; }

    /// <inheritdoc />
    public Task Register(AggregateRootsClient aggregateRoots, CancellationToken cancellationToken)
        => aggregateRoots.Register(Bindings.Select(WithDefaultAliasIfNotSet), cancellationToken);

    void AddToContainer(TenantId tenantId, IServiceCollection serviceCollection)
    {
        foreach (var type in Types)
        {
            serviceCollection.AddSingleton(
                typeof(IAggregateOf<>).MakeGenericType(type),
                serviceProvider => Activator.CreateInstance(
                    typeof(AggregateOf<>).MakeGenericType(type),
                    serviceProvider.GetService<IAggregates>()) ?? throw new CouldNotCreateAggregateOf(type, tenantId));
        }
    }

    static AggregateRootType WithDefaultAliasIfNotSet((AggregateRootType, Type) binding)
    {
        var (aggregateRootType, type) = binding;
        if (!aggregateRootType.HasAlias)
        {
            aggregateRootType = new AggregateRootType(aggregateRootType.Id, aggregateRootType.Generation, type.Name);
        }
        return aggregateRootType;
    }
}
