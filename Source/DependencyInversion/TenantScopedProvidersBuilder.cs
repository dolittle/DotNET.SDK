// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents the builder for <see cref="ITenantScopedProviders"/>.
    /// </summary>
    public class TenantScopedProvidersBuilder
    {
        readonly List<ConfigureTenantServices> _configureServicesForTenantCallbacks = new List<ConfigureTenantServices>();

        HashSet<TenantId> _tenants = new HashSet<TenantId>();
        IServiceProvider _rootProvider;

        /// <summary>
        /// Populates the root <see cref="IServiceCollection"/> with al the given <see cref="ServiceDescriptor"/> services.
        /// </summary>
        /// <param name="provider">The root <see cref="IServiceProvider"/>.</param>
        /// <returns>The builder for continuation.</returns>
        public TenantScopedProvidersBuilder WithRoot(IServiceProvider provider)
        {
            _rootProvider = provider;
            return this;
        }

        /// <summary>
        /// Sets the tenants .
        /// </summary>
        /// <param name="tenants">The <see cref="IEnumerable{T}"/> of <see cref="TenantId"/>.</param>
        /// <returns>The builder for continuation.</returns>
        public TenantScopedProvidersBuilder WithTenants(IEnumerable<TenantId> tenants)
        {
            _tenants = new HashSet<TenantId>(tenants);
            return this;
        }

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
        /// <returns>The built <see cref="ITenantScopedProviders"/>.</returns>
        public ITenantScopedProviders Build()
            => new TenantScopedProviders(_tenants.ToDictionary(tenant => tenant, CreateTenantContainer));

        IServiceProvider CreateTenantContainer(TenantId tenant)
        {
            _rootProvider ??= new ServiceCollection().BuildServiceProvider();
            var containerBuilder = new ContainerBuilder();
            var services = new ServiceCollection();
            foreach (var configure in _configureServicesForTenantCallbacks)
            {
                configure?.Invoke(tenant, services);
            }

            containerBuilder.Populate(services);
            containerBuilder.RegisterInstance(tenant);
            var container = containerBuilder.Build();

            var rootScope = container.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(new ServiceScopeFactory(_rootProvider.GetRequiredService<IServiceScopeFactory>(), container)).As<IServiceScopeFactory>();
                builder.RegisterSource(new UnknownServiceOnTenantContainerRegistrationSource(_rootProvider));
            });

            return new AutofacServiceProvider(rootScope);
        }
    }
}