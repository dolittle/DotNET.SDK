// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources;

/// <summary>
/// Exception that gets thrown when attempting to get resources for a <see cref="TenantId"/> that the client has not fetched resources for.
/// </summary>
public class ResourcesNotFetchedForTenant : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourcesNotFetchedForTenant"/> class.
    /// </summary>
    /// <param name="tenant">The tenant that resources has not been fetched for.</param>
    public ResourcesNotFetchedForTenant(TenantId tenant)
        : base($"No resources fetched for {tenant}. Please make sure this tenant is configured in the Runtime.")
    {
    }
}
