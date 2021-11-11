// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Exception that gets thrown when getting a resource for a tenant failed.
    /// </summary>
    public class FailedToGetResource : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToGetResource"/> class.
        /// </summary>
        /// <param name="resource">The reason for why it failed.</param>
        /// <param name="tenantId">The <see cref="TenantId"/> to get the resource for.</param>
        /// <param name="reason">The reason why it failed.</param>
        public FailedToGetResource(string resource, TenantId tenantId, string reason)
            : base($"Failed to get resource {resource} for tenant {tenantId.Value} because {reason}")
        {
        }
    }
}