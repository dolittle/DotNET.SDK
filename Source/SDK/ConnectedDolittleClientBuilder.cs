// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Internal;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK
{
    
#pragma warning disable CS1591
    public class ConnectedDolittleClientBuilder
    {
        readonly Internal.DolittleClientParams _clientParams;
        IServiceCollection _serviceCollection = new ServiceCollection();
        Action<TenantId, IServiceCollection> _configureServicesForTenant;

        public ConnectedDolittleClientBuilder(DolittleClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        protected ConnectedDolittleClientBuilder(DolittleClientParams clientParams, IServiceCollection serviceCollection, Action<TenantId, IServiceCollection> configureServicesForTenant)
        {
            _clientParams = clientParams;
            _serviceCollection = serviceCollection;
            _configureServicesForTenant = configureServicesForTenant;
        }
#pragma warning restore CS1591

        /// <summary>
        /// Sets the preconfigured <see cref="IServiceCollection"/> that's to be added to the <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="services">The given <see cref="IServiceCollection"/>.</param>
        /// <returns>The client builder for continuation.</returns>
        public ConnectedDolittleClientBuilder WithServices(IServiceCollection services)
        {
            _serviceCollection = services;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Action"/> callback that configures services for tenant.
        /// </summary>
        /// <param name="configureServicesForTenant">The <see cref="Action"/> callback for configuring an <see cref="IServiceCollection"/> tied to a <see cref="TenantId"/>.</param>
        /// <returns>The client builder for continuation.</returns>
        public ConnectedDolittleClientBuilder WithTenantServices(Action<TenantId, IServiceCollection> configureServicesForTenant)
        {
            _configureServicesForTenant = configureServicesForTenant;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Action"/> callback that configures services for tenant.
        /// </summary>
        /// <param name="serviceProviderFactory">The non-Dolittle <see cref="IServiceProviderFactory{TContainerBuilder}"/>.</param>
        /// <param name="configureContainer">The <see cref="Action{T}"/> callback for configuring the <typeparamref name="TContainerBuilder"/>.</param>
        /// <returns>The client builder for continuation.</returns>
        public ConnectedDolittleClientBuilder<TContainerBuilder> WithNonDolittleServiceProviderFactory<TContainerBuilder>(
            IServiceProviderFactory<TContainerBuilder> serviceProviderFactory,
            Action<TContainerBuilder> configureContainer = default)
            => new ConnectedDolittleClientBuilder<TContainerBuilder>(_clientParams, _serviceCollection, _configureServicesForTenant, serviceProviderFactory, configureContainer);

        /// <summary>
        /// Builds the <see cref="DolittleClient"/>.
        /// </summary>
        /// <param name="serviceProviderFactory">The optional <see cref="ServiceProviderFactory"/>.</param>
        /// <returns>The built <see cref="DolittleClient"/>.</returns>
        public virtual DolittleClient Build(ServiceProviderFactory serviceProviderFactory = default)
            => Build<object>(serviceProviderFactory, null, null);

        /// <summary>
        /// Builds the <see cref="DolittleClient"/>.
        /// </summary>
        /// <param name="serviceProviderFactory"></param>
        /// <param name="nonDolittleServiceProviderFactory"></param>
        /// <param name="configureNonDolittleContainer"></param>
        /// <typeparam name="TContainerBuilder"></typeparam>
        /// <returns>The built <see cref="DolittleClient"/>.</returns>
        protected DolittleClient Build<TContainerBuilder>(ServiceProviderFactory serviceProviderFactory, IServiceProviderFactory<TContainerBuilder> nonDolittleServiceProviderFactory, Action<TContainerBuilder> configureNonDolittleContainer)
        {
            serviceProviderFactory ??= new ServiceProviderFactory();
            var builder = serviceProviderFactory.CreateBuilder(_serviceCollection);
            if (nonDolittleServiceProviderFactory != default)
            {
                builder.UseServiceProviderFactory(nonDolittleServiceProviderFactory, configureNonDolittleContainer);
            }

            if (_configureServicesForTenant != default)
            {
                builder.AddTenantServices(_configureServicesForTenant);
            }

            foreach (var tenant in _clientParams.Tenants)
            {
                builder.AddTenant(tenant.Id);
            }

            // Create DolilttleClient
            // Populate root container with IDolittleClient
            // Return the DolittleClient
        }
    }
}