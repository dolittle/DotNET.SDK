// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Defines a system that knows about the Resources provided by the Runtime.
    /// </summary>
    public interface IResourcesBuilder
    {
        /// <summary>
        /// Gets the <see cref="IResources"/> for a specific tenant.
        /// </summary>
        /// <param name="tenant">The Tenant to get the <see cref="IResources"/> for.</param>
        /// <returns>The <see cref="IResources"/>.</returns>
        IResources ForTenant(TenantId tenant);
    }
}