// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents the builder for <see cref="IContainer"/>.
    /// </summary>
    public class ContainerBuilder
    {
        readonly IServiceCollection _rootServices = new ServiceCollection();
        readonly HashSet<TenantId> _tenants = new HashSet<TenantId>();
        readonly List<Action<TenantId, IServiceCollection>> _configureServicesForTenantCalbacks = new List<Action<TenantId, IServiceCollection>>();

        /// <summary>
        /// Populates the root <see cref="IServiceCollection"/> with al the given <see cref="ServiceDescriptor"/> services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to populate the root service collec</param>
        public void PopulateRootContainer(IServiceCollection services)
        {
            foreach (var serviceDescriptor in services)
            {
                _rootServices.Add(serviceDescriptor);
            }
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
        {
            _configureServicesForTenantCalbacks.Add(configureServicesForTenant);
        }

        /// <summary>
        /// Builds the <see cref="IContainer"/>.
        /// </summary>
        /// <returns>The built <see cref="IContainer"/>.</returns>
        public IContainer Build()
        {
            var rootProvider = _rootServices.BuildServiceProvider();
            return new Container(
                _tenants.ToDictionary(
                    tenant => tenant,
                    tenant => CreateTenantScopedServicesFromRoot(tenant, rootProvider))
                .ToDictionary<KeyValuePair<TenantId, IServiceCollection>, TenantId, IServiceProvider>(
                    _ => _.Key,
                    _ => _.Value.BuildServiceProvider()), rootProvider);
        }


        IServiceCollection CreateTenantScopedServicesFromRoot(TenantId tenant, IServiceProvider rootProvider)
        {
            var tenantServices = new ServiceCollection();
            foreach (var serviceDescriptor in _rootServices)
            {
                tenantServices.Add(new ServiceDescriptor(
                    serviceDescriptor.ServiceType,
                    _ => rootProvider.GetService(serviceDescriptor.ServiceType),
                    serviceDescriptor.Lifetime));
            }

            foreach (var configure in _configureServicesForTenantCalbacks)
            {
                configure?.Invoke(tenant, tenantServices);
            }

            tenantServices.AddSingleton(tenant);
            return tenantServices;
        }
    }
}