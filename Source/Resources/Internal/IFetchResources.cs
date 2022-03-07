// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources.Internal;

/// <summary>
/// Defines a system that can create instances of <see cref="IResourcesBuilder"/> by fetching resources from the Runtime.
/// </summary>
public interface IFetchResources
{
    /// <summary>
    /// Creates a <see cref="IResourcesBuilder"/> by fetching resources for the provided <see cref="Tenant">tenants</see>.
    /// </summary>
    /// <param name="tenants">The tenants to fetch resources for.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="IResourcesBuilder"/>.</returns>
    Task<IResourcesBuilder> FetchResourcesFor(IEnumerable<Tenant> tenants, CancellationToken cancellationToken = default);
}
