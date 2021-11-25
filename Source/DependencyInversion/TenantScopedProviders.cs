// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="ITenantScopedProviders" />.
    /// </summary>
    public class TenantScopedProviders : ITenantScopedProviders
    {
        readonly IReadOnlyDictionary<TenantId, IServiceProvider> _serviceProviders;
        readonly IServiceProvider _rootContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantScopedProviders"/> class.
        /// </summary>
        /// <param name="tenantScopedServiceProviders">The <see cref="IDictionary{TKey,TValue}"/> of <see cref="IServiceProvider"/> per <see cref="TenantId" />.</param>
        /// <param name="rootContainer">The root <see cref="IServiceProvider"/>.</param>
        public TenantScopedProviders(IDictionary<TenantId, IServiceProvider> tenantScopedServiceProviders, IServiceProvider rootContainer)
        {
            _rootContainer = rootContainer;
            _serviceProviders = new ReadOnlyDictionary<TenantId, IServiceProvider>(tenantScopedServiceProviders);
        }

        /// <inheritdoc />
        public IServiceProvider ForTenant(TenantId tenant)
        {
            if (!_serviceProviders.TryGetValue(tenant, out var provider))
            {
                throw new MissingServiceProviderForTenant(tenant);
            }

            return provider;
        }
    }
}
