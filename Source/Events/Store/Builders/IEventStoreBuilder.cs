// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Events.Store.Builders;

/// <summary>
/// Defines a builder for <see cref="IEventStore"/>.
/// </summary>
public interface IEventStoreBuilder
{
    /// <summary>
    /// Creates an <see cref="IEventStore"/> for the given tenant.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId">tenant</see> to create the event store for.</param>
    /// <returns>An <see cref="IEventStore"/>.</returns>
    IEventStore ForTenant(TenantId tenantId);
}