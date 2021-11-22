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
    /// Represents an implementation of <see cref="IContainer" />.
    /// </summary>
    public class Container : IContainer
    {
        readonly IReadOnlyDictionary<TenantId, IServiceProvider> _serviceProviders;
        readonly IServiceProvider _rootContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="tenantScopedServiceProviders">The <see cref="IDictionary{TKey,TValue}"/> of <see cref="IServiceProvider"/> per <see cref="TenantId" />.</param>
        /// <param name="rootContainer">The root <see cref="IServiceProvider"/>.</param>
        public Container(IDictionary<TenantId, IServiceProvider> tenantScopedServiceProviders, IServiceProvider rootContainer)
        {
            _rootContainer = rootContainer;
            _serviceProviders = new ReadOnlyDictionary<TenantId, IServiceProvider>(tenantScopedServiceProviders);
        }

        /// <inheritdoc/>
        public object Get(Type service, ExecutionContext context)
            => Get(service, context.Tenant);

        /// <inheritdoc />
        public object Get(Type service, TenantId tenant)
        {
            var result = GetProviderFor(tenant).GetService(service);
            if (result is null)
            {
                throw new MissingServiceForTenant(service, tenant);
            }

            return result;
        }

        /// <inheritdoc/>
        public T Get<T>(ExecutionContext context)
            where T : class
            => Get(typeof(T), context.Tenant) as T;

        /// <inheritdoc />
        public T Get<T>(TenantId tenant)
            where T : class
            => Get(typeof(T), tenant) as T;

        /// <inheritdoc />
        public IServiceProvider GetProviderFor(ExecutionContext executionContext)
            => GetProviderFor(executionContext.Tenant);

        /// <inheritdoc />
        public IServiceProvider GetProviderFor(TenantId tenant)
        {
            if (!_serviceProviders.TryGetValue(tenant, out var provider))
            {
                throw new MissingServiceProviderForTenant(tenant);
            }

            return provider;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
            => _rootContainer.GetService(serviceType);
    }
}
