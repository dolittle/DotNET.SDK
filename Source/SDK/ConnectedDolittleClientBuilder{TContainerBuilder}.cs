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
    public class ConnectedDolittleClientBuilder<TContainerBuilder> : ConnectedDolittleClientBuilder
    {
        readonly IServiceProviderFactory<TContainerBuilder> _nonDolittleServiceProviderFactory;
        readonly Action<TContainerBuilder> _configureNonDolittleContainer;

        public ConnectedDolittleClientBuilder(DolittleClientParams clientParams,
            IServiceCollection serviceCollection,
            Action<TenantId, IServiceCollection> configureServicesForTenant,
            IServiceProviderFactory<TContainerBuilder> nonDolittleServiceProviderFactory,
            Action<TContainerBuilder> configureNonDolittleContainer)
#pragma warning restore CS1591
            : base(clientParams, serviceCollection, configureServicesForTenant)
        {
            _nonDolittleServiceProviderFactory = nonDolittleServiceProviderFactory;
            _configureNonDolittleContainer = configureNonDolittleContainer;
        }

        /// <inheritdoc />
        public override DolittleClient Build(ServiceProviderFactory serviceProviderFactory = default)
            => Build(serviceProviderFactory, _nonDolittleServiceProviderFactory, _configureNonDolittleContainer);
    }
}