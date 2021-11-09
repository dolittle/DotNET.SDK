// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Tenancy
{
    /// <summary>
    /// Represents a tenant.
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class.
        /// </summary>
        /// <param name="id">The <see cref="TenantId"/>.</param>
        public Tenant(TenantId id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the id of the tenant.
        /// </summary>
        public TenantId Id { get; }
    }
}