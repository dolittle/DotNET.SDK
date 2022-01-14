// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
    }

    /// <inheritdoc />
    public ConfigureTenantServices AddTenantScopedServices(IAggregatesBuilder aggregatesBuilder)
        => (tenant, serviceCollection) => AddToContainer(tenant, serviceCollection, aggregatesBuilder);

    /// <inheritdoc />
    public Task Register(AggregateRootsClient aggregateRoots, CancellationToken cancellationToken)
        => aggregateRoots.Register(All, cancellationToken);

    void AddToContainer(TenantId tenantId, IServiceCollection serviceCollection, IAggregatesBuilder aggregatesBuilder)
    {
        foreach (var type in Types)
        {
            serviceCollection.AddSingleton(typeof(IAggregateOf<>).MakeGenericType(type), _ =>
            {
                var aggregates = aggregatesBuilder.ForTenant(tenantId);
                return Activator.CreateInstance(typeof(AggregateOf<>).MakeGenericType(type), aggregates) ?? throw new CouldNotCreateAggregateOf(type, tenantId);
            });
        }
    }
}
