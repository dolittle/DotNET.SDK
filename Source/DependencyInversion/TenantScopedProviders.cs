// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac.Extensions.DependencyInjection;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ITenantScopedProviders" />.
/// </summary>
public class TenantScopedProviders : ITenantScopedProviders
{
    readonly IReadOnlyDictionary<TenantId, AutofacServiceProvider> _serviceProviders;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantScopedProviders"/> class.
    /// </summary>
    /// <param name="tenantScopedServiceProviders">The <see cref="IDictionary{TKey,TValue}"/> of <see cref="IServiceProvider"/> per <see cref="TenantId" />.</param>
    public TenantScopedProviders(IDictionary<TenantId, AutofacServiceProvider> tenantScopedServiceProviders)
    {
        _serviceProviders = new ReadOnlyDictionary<TenantId, AutofacServiceProvider>(tenantScopedServiceProviders);
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

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var (_, provider) in _serviceProviders)
        {
            provider.Dispose();
        }
    }
}
