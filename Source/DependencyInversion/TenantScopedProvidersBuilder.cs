// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents the builder for <see cref="ITenantScopedProviders"/>.
/// </summary>
public class TenantScopedProvidersBuilder
{
    readonly List<ConfigureTenantServices> _configureServicesForTenantCallbacks = new();

    /// <summary>
    /// Adds a callback that configures services for tenant.
    /// </summary>
    /// <param name="configureServicesForTenant">The <see cref="ConfigureTenantServices"/> callback.</param>
    /// <returns>The builder for continuation.</returns>
    public TenantScopedProvidersBuilder AddTenantServices(ConfigureTenantServices configureServicesForTenant)
    {
        _configureServicesForTenantCallbacks.Add(configureServicesForTenant);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="ITenantScopedProviders"/>.
    /// </summary>
    /// <param name="tenants">The list of <see cref="TenantId"/> to make tenant scoped <see cref="IServiceProvider"/> for.</param>
    /// <param name="createTenantContainer">The callback for creating tenant containers.</param>
    /// <returns>The built <see cref="ITenantScopedProviders"/>.</returns>
    public ITenantScopedProviders Build(HashSet<TenantId> tenants, CreateTenantContainer createTenantContainer)
        => new TenantScopedProviders(tenants.ToDictionary(tenant => tenant, tenant => CreateTenantContainer(tenant, createTenantContainer)));

    IServiceProvider CreateTenantContainer(TenantId tenant, CreateTenantContainer createTenantContainer)
    {
        var services = new ServiceCollection();
        foreach (var configure in _configureServicesForTenantCallbacks)
        {
            configure?.Invoke(tenant, services);
        }
        services.AddSingleton(tenant);
        return createTenantContainer(services);
    }
}
