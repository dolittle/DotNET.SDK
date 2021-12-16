// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Defines a build for <see cref="IAggregates"/> for tenants.
/// </summary>
public interface IAggregatesBuilder
{
    /// <summary>
    /// Gets an <see cref="IAggregates"/> for a specific <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <returns>The <see cref="IAggregates"/> for the specified <see cref="TenantId"/>.</returns>
    IAggregates ForTenant(TenantId tenant);
}
