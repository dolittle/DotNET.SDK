// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.Builders
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregatesBuilder"/>.
    /// </summary>
    public class AggregatesBuilder : IAggregatesBuilder
    {
        readonly IEventTypes _eventTypes;
        readonly IAggregateRoots _aggregateRoots;
        readonly IEventStoreBuilder _eventStoreBuilder;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatesBuilder"/> class.
        /// </summary>
        /// <param name="eventStoreBuilder">The <see cref="IEventStoreBuilder" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public AggregatesBuilder(IEventStoreBuilder eventStoreBuilder, IEventTypes eventTypes, IAggregateRoots aggregateRoots, ILoggerFactory loggerFactory)
        {
            _eventTypes = eventTypes;
            _aggregateRoots = aggregateRoots;
            _eventStoreBuilder = eventStoreBuilder;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IAggregates ForTenant(TenantId tenant)
            => new Aggregates(_eventStoreBuilder.ForTenant(tenant), _eventTypes, _aggregateRoots, _loggerFactory);
    }
}