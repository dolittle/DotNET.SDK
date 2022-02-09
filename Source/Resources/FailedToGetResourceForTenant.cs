// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources;

/// <summary>
/// Exception that gets thrown when getting a resource for a tenant failed.
/// </summary>
public class FailedToGetResourceForTenant : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedToGetResourceForTenant"/> class.
    /// </summary>
    /// <param name="resource">The name of the resource that failed.</param>
    /// <param name="tenant">The tenant the resource was attempted to create for.</param>
    /// <param name="reason">The reason why it failed.</param>
    public FailedToGetResourceForTenant(ResourceName resource, TenantId tenant, string reason)
        : base($"Failed to get resource {resource} for tenant {tenant} because {reason}")
    {
    }
}
