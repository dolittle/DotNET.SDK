// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Defines a collection of aggregate roots that hasn't been registered with the Runtime yet.
/// </summary>
public interface IUnregisteredAggregateRoots : IAggregateRootTypes
{
    /// <summary>
    /// Registers the aggregate roots by with the Runtime.
    /// </summary>
    /// <param name="aggregateRoots">The <see cref="Internal.AggregateRootsClient"/>.</param>
    /// <param name="tenantScopedProvidersBuilder">The <see cref="TenantScopedProvidersBuilder"/>.</param>
    /// <param name="aggregatesBuilder">The <see cref="IAggregatesBuilder"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Register(
        AggregateRootsClient aggregateRoots,
        TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
        IAggregatesBuilder aggregatesBuilder,
        CancellationToken cancellationToken);
}
