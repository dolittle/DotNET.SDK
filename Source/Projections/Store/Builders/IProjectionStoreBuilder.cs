// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Projections.Store.Builders;

/// <summary>
/// Defines a builder for <see cref="IProjectionStore"/>.
/// </summary>
public interface IProjectionStoreBuilder
{
    /// <summary>
    /// Gets the projection store <see cref="IProjectionStore"/> for the given tenant.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId">tenant</see> to get projections for.</param>
    /// <returns>An <see cref="IProjectionStore"/>.</returns>
    IProjectionStore ForTenant(TenantId tenantId);
}
