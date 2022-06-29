// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
    /// <param name="rootServiceProvider">The root <see cref="IServiceProvider"/>.</param>
    /// <param name="tenants">The list of <see cref="TenantId"/> to make tenant scoped <see cref="IServiceProvider"/> for.</param>
    /// <returns>The built <see cref="ITenantScopedProviders"/>.</returns>
    public ITenantScopedProviders Build(IServiceProvider rootServiceProvider, HashSet<TenantId> tenants)
        => new TenantScopedProviders(tenants.ToDictionary(tenant => tenant, tenant => CreateTenantContainer(tenant, rootServiceProvider)));

    (AutofacServiceProvider, IDisposable) CreateTenantContainer(TenantId tenant, IServiceProvider rootServiceProvider)
    {
        var containerBuilder = new ContainerBuilder();
        var services = new ServiceCollection();
        foreach (var configure in _configureServicesForTenantCallbacks)
        {
            configure?.Invoke(tenant, services);
        }

        if (services.Any(_ => _.Lifetime == ServiceLifetime.Singleton && _.ImplementationInstance == null))
        {
            throw new CannotRegisterSingletonDependenciesOnTenantScopedContainer();
        }

        containerBuilder.Populate(services);
        containerBuilder.RegisterInstance(tenant);
        var container = containerBuilder.Build();
        var rootScope = container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(new ServiceScopeFactory(rootServiceProvider.GetRequiredService<IServiceScopeFactory>(), container)).As<IServiceScopeFactory>();
            builder.RegisterSource(new UnknownServiceOnTenantContainerRegistrationSource(rootServiceProvider));
        });

        return (new AutofacServiceProvider(rootScope), container);
    }
}
