// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;
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
        readonly HashSet<TenantId> _tenants = new HashSet<TenantId>();
        readonly List<ConfigureTenantServices> _configureServicesForTenantCallbacks = new List<ConfigureTenantServices>();

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
        /// <param name="configureServicesForTenant">The <see cref="ConfigureTenantServices"/> callback.</param>
        public void AddTenantServices(ConfigureTenantServices configureServicesForTenant)
            => _configureServicesForTenantCallbacks.Add(configureServicesForTenant);

        /// <summary>
        /// Builds the <see cref="ITenantScopedProviders"/>.
        /// </summary>
        /// <returns>The built <see cref="ITenantScopedProviders"/>.</returns>
        public ITenantScopedProviders Build()
            => new TenantScopedProviders(_tenants.ToDictionary(tenant => tenant, CreateTenantContainer), _rootProvider);

        IServiceProvider CreateTenantContainer(TenantId tenant)
        {
            var containerBuilder = new ContainerBuilder();
            var services = new ServiceCollection();
            foreach (var configure in _configureServicesForTenantCallbacks)
            {
                configure?.Invoke(tenant, services);
            }

            containerBuilder.Populate(services);

            containerBuilder.RegisterSource(new UnknownServiceOnTenantContainerRegistrationSource(_rootProvider));
            containerBuilder.RegisterInstance(tenant);
            containerBuilder
                .Register(_ => new ServiceScopeFactory(_rootProvider.GetRequiredService<IServiceScopeFactory>(), _.Resolve<ILifetimeScope>()))
                .As<IServiceScopeFactory>();
            containerBuilder.RegisterInstance(new RootProviderWrapper(_rootProvider));
            containerBuilder.ComponentRegistryBuilder.Registered += (sender, args) =>
            {
                // The PipelineBuilding event fires just before the pipeline is built, and
                // middleware can be added inside it.
                args.ComponentRegistration.PipelineBuilding += (sender2, pipeline) =>
                {
                    pipeline.Use(PipelinePhase.RegistrationPipelineStart, (context, next) =>
                    {
                        if (context.Service is IServiceWithType swt)
                        {
                            Console.WriteLine($"In my middleware {swt.ServiceType}");
                        }

                        next(context);
                    });
                };
            };
            return new AutofacServiceProvider(containerBuilder.Build());

            // Console.WriteLine(provider.GetRequiredService<IServiceScopeFactory>().GetType());
            // return provider;
        }
    }
}