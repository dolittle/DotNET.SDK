// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredAggregateRoots"/> and <see cref="AggregateRootTypes"/>.
/// </summary>
public class UnregisteredAggregateRoots : AggregateRootTypes, IUnregisteredAggregateRoots
{
    /// <inheritdoc />
    public async Task Register(
        AggregateRootsClient aggregateRoots,
        TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
        IAggregatesBuilder aggregatesBuilder,
        CancellationToken cancellationToken)
    {
        await aggregateRoots.Register(All, cancellationToken).ConfigureAwait(false);
        foreach (var type in Types)
        {
            tenantScopedProvidersBuilder.AddTenantServices((tenant, services) =>
            {
                var aggregates = aggregatesBuilder.ForTenant(tenant);
                var aggregateOfInstance = Activator.CreateInstance(typeof(AggregateOf<>).MakeGenericType(type), aggregates) ?? throw new CouldNotCreateAggregateOf(type, tenant);
                services.AddSingleton(typeof(IAggregateOf<>).MakeGenericType(type), aggregateOfInstance);
            });
        }
    }
}
