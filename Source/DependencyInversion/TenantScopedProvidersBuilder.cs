// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
    /// <param name="rootServiceProvider">The root <see cref="IServiceProvider"/>.</param>
    /// <param name="tenants">The list of <see cref="TenantId"/> to make tenant scoped <see cref="IServiceProvider"/> for.</param>
    /// <returns>The built <see cref="ITenantScopedProviders"/>.</returns>
    public ITenantScopedProviders Build(IServiceProvider rootServiceProvider, HashSet<TenantId> tenants)
    {
        var tenantScopedContainerCreators = rootServiceProvider.GetService<IEnumerable<ICanCreateTenantScopedContainer>>()?.ToArray() ?? Array.Empty<ICanCreateTenantScopedContainer>();
        var tenantScopedContainerCreator = tenantScopedContainerCreators.Length switch
        {
            0 => new TenantScopedContainerCreator(),
            1 => tenantScopedContainerCreators[0],
            _ => throw new MultipleTenantScopedContainerCreatorsUsed()
        };
        return new TenantScopedProviders(tenants.ToDictionary(tenant => tenant, tenant => CreateTenantContainer(tenant, rootServiceProvider, tenantScopedContainerCreator)));
    }

    IServiceProvider CreateTenantContainer(TenantId tenant, IServiceProvider rootServiceProvider, ICanCreateTenantScopedContainer tenantScopedContainerCreator)
    {
        var logger = rootServiceProvider.GetService<ILogger<TenantScopedProvidersBuilder>>() ?? NullLogger<TenantScopedProvidersBuilder>.Instance;
        if (!tenantScopedContainerCreator.CanCreateFrom(rootServiceProvider))
        {
            throw new TenantScopedContainerCreatorCannotCreateFromRootServiceProvider(tenantScopedContainerCreator, rootServiceProvider);
        }

        var services = new ServiceCollection();
        foreach (var configure in _configureServicesForTenantCallbacks)
        {
            configure?.Invoke(tenant, services);
        }
        services.AddSingleton(tenant);
        if (tenantScopedContainerCreator is not TenantScopedContainerCreator)
        {
            return tenantScopedContainerCreator.Create(rootServiceProvider, services);
        }

        logger.LogWarning("Using the default TenantScopedContainerCreator using Microsoft's dependency injection framework. This is limited as it does not support singleton services that are not registered as a value");
        if (services.Any(_ => _.Lifetime == ServiceLifetime.Singleton && _.ImplementationInstance == null))
        {
            throw new CannotRegisterSingletonDependenciesOnTenantScopedContainer();
        }
        return tenantScopedContainerCreator.Create(rootServiceProvider, services);
    }
}
