// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Defines a resource for a specific Tenant.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the <see cref="ResourceName"/> of the resource.
        /// </summary>
        ResourceName Name { get; }

        /// <summary>
        /// Gets the <see cref="TenantId"/> that this resource belongs to.
        /// </summary>
        TenantId Tenant { get; }
    }
}
