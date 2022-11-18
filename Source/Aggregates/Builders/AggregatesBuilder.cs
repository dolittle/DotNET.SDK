// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregatesBuilder"/>.
/// </summary>
public class AggregatesBuilder : IAggregatesBuilder
{
    readonly IEventTypes _eventTypes;
    readonly Func<TenantId, IServiceProvider> _getServiceProvider;
    readonly IEventStoreBuilder _eventStoreBuilder;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatesBuilder"/> class.
    /// </summary>
    /// <param name="eventStoreBuilder">The <see cref="IEventStoreBuilder" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="getServiceProvider">The <see cref="Func{TResult}"/> for getting the tenant scoped <see cref="IServiceProvider"/> for a <see cref="TenantId"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public AggregatesBuilder(IEventStoreBuilder eventStoreBuilder, IEventTypes eventTypes, Func<TenantId, IServiceProvider> getServiceProvider, ILoggerFactory loggerFactory)
    {
        _eventTypes = eventTypes;
        _getServiceProvider = getServiceProvider;
        _eventStoreBuilder = eventStoreBuilder;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IAggregates ForTenant(TenantId tenant)
        => new Aggregates(_getServiceProvider(tenant));
}
