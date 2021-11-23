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
    /// Represents the builder for <see cref="IContainer"/>.
    /// </summary>
    public class ContainerBuilder
    {
        readonly HashSet<TenantId> _tenants = new HashSet<TenantId>();
        readonly List<Action<TenantId, IServiceCollection>> _configureServicesForTenantCallbacks = new List<Action<TenantId, IServiceCollection>>();

        IServiceProvider _rootProvider = new ServiceCollection().BuildServiceProvider();

        /// <summary>
        /// Populates the root <see cref="IServiceCollection"/> with al the given <see cref="ServiceDescriptor"/> services.
        /// </summary>
        /// <param name="provider">The root <see cref="IServiceProvider"/>.</param>
        public void UseRootProvider(IServiceProvider provider)
        {
            _rootProvider = provider;
        }

        /// <summary>
        /// Adds a tenant.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId"/> of the tenant.</param>
        public void AddTenant(TenantId tenant)
            => _tenants.Add(tenant);

        /// <summary>
        /// Adds a callback that configures services for tenant.
        /// </summary>
        /// <param name="configureServicesForTenant">The <see cref="Action{T}"/> callback for configuring an <see cref="IServiceCollection"/> tied to a <see cref="TenantId"/>.</param>
        public void AddTenantServices(Action<TenantId, IServiceCollection> configureServicesForTenant)
            => _configureServicesForTenantCallbacks.Add(configureServicesForTenant);

        /// <summary>
        /// Builds the <see cref="IContainer"/>.
        /// </summary>
        /// <returns>The built <see cref="IContainer"/>.</returns>
        public IContainer Build()
            => new Container(_tenants.ToDictionary(tenant => tenant, tenant => CreateTenantContainer(tenant, new UnknownServiceOnTenantContainerRegistrationSource(_rootProvider))), _rootProvider);

        IServiceProvider CreateTenantContainer(TenantId tenant, UnknownServiceOnTenantContainerRegistrationSource unknownServiceOnTenantContainerRegistrationSource)
        {
            var services = new ServiceCollection();
            var serviceProviderFactory = new AutofacServiceProviderFactory(_ => _.RegisterSource(unknownServiceOnTenantContainerRegistrationSource));

            foreach (var configure in _configureServicesForTenantCallbacks)
            {
                configure?.Invoke(tenant, services);
            }

            services.AddSingleton(tenant);
            var containerBuilder = serviceProviderFactory.CreateBuilder(services);
            return serviceProviderFactory.CreateServiceProvider(containerBuilder);
        }
    }
}