// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources;

/// <summary>
/// Represents an implementation of <see cref="IResourcesBuilder"/>.
/// </summary>
public class ResourcesBuilder : IResourcesBuilder
{
    readonly IDictionary<TenantId, IResources> _tenantResources;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourcesBuilder"/> class.
    /// </summary>
    /// <param name="tenantResources">The fetched resources per tenant.</param>
    public ResourcesBuilder(IDictionary<TenantId, IResources> tenantResources)
    {
        _tenantResources = tenantResources;
    }

    /// <inheritdoc />
    public IResources ForTenant(TenantId tenant)
    {
        if (!_tenantResources.TryGetValue(tenant, out var resources))
        {
            throw new ResourcesNotFetchedForTenant(tenant);
        }

        return resources;
    }
}
