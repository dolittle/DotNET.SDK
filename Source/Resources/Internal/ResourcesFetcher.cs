// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Resources.Internal;

/// <summary>
/// Represents an implementation of <see cref="IFetchResources"/>.
/// </summary>
public class ResourcesFetcher : IFetchResources
{
    public Task<IResourcesBuilder> FetchResourcesFor(IEnumerable<Tenant> tenants, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
}
