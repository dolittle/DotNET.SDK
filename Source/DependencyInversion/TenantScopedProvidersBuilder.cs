// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents the builder for <see cref="ITenantScopedProviders"/>.
/// </summary>
public class TenantScopedProvidersBuilder
{
    readonly List<ConfigureTenantServices> _configureServicesForTenantCallbacks = [];
    readonly IServiceProvider _serviceProvider;
    readonly CreateTenantServiceProvider _factory;

    /// <summary>
    /// Initialises a new instance of the <see cref="TenantScopedProvidersBuilder"/> class.
    /// </summary>
    /// <param name="serviceProvider">The root <see cref="IServiceProvider"/> to use.</param>
    /// <param name="factory">The factory to </param>
    public TenantScopedProvidersBuilder(IServiceProvider serviceProvider, CreateTenantServiceProvider factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    /// <summary>
    /// Adds a callback that configures services for tenant.
    /// </summary>
    /// <param name="configureServicesForTenant">The <see cref="ConfigureTenantServices"/> callback.</param>
    /// <returns>The builder for continuation.</returns>
    public TenantScopedProvidersBuilder AddTenantServices(ConfigureTenantServices? configureServicesForTenant)
    {
        if (configureServicesForTenant is not null)
        {
            _configureServicesForTenantCallbacks.Add(configureServicesForTenant);
        }

        return this;
    }

    /// <summary>
    /// Builds the <see cref="ITenantScopedProviders"/>.
    /// </summary>
    /// <param name="tenants">The set of <see cref="TenantId"/> to make tenant scoped <see cref="IServiceProvider"/> for.</param>
    /// <returns>The built <see cref="ITenantScopedProviders"/>.</returns>
    public ITenantScopedProviders Build(IImmutableSet<TenantId> tenants)
        => new TenantScopedProviders(tenants.ToDictionary(tenant => tenant, CreateTenantContainer));

    IServiceProvider CreateTenantContainer(TenantId tenant)
    {
        var services = new ServiceCollection();
        DiscoverTenantScopedServices(services);
        foreach (var configure in _configureServicesForTenantCallbacks)
        {
            configure?.Invoke(tenant, services);
        }
        services.AddSingleton(tenant);
        return _factory(_serviceProvider, tenant, services);
    }

    static void DiscoverTenantScopedServices(IServiceCollection services)
    {
        ForAllAllScannedAssemblies(assembly => services
            .Add(GetExportedTypes(assembly).SelectMany(_ => _.GetTenantScopedServiceDescriptors())));
    }

    static IEnumerable<Type> GetExportedTypes(Assembly assembly)
    {
        try
        {
            return assembly.ExportedTypes;
        }
        catch
        {
            return [];
        }
    }
    
    static void ForAllAllScannedAssemblies(Action<Assembly> registerAllFromAssembly)
    {
        foreach (var assembly in GetAllAssemblies())
        {
            registerAllFromAssembly(assembly);
        }
    }

    static IEnumerable<Assembly> GetAllAssemblies()
    {
        return AssemblyFinder.FindAssemblies(
            _ => true,
            false);
    }
}
