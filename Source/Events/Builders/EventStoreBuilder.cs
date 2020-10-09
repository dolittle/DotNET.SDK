// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Builders
{
    /// <summary>
    /// Represents a builder for building <see cref="EventStore"/>.
    /// </summary>
    public class EventStoreBuilder
    {
        readonly IPerformMethodCalls _caller;
        readonly IEventConverter _eventConverter;
        readonly ExecutionContext _executionContext;
        readonly IResolveCallContext _callContextResolver;
        readonly IEventTypes _eventTypes;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreBuilder"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventStoreBuilder(
            IPerformMethodCalls caller,
            IEventConverter eventConverter,
            ExecutionContext executionContext,
            IResolveCallContext callContextResolver,
            IEventTypes eventTypes,
            ILoggerFactory loggerFactory)
        {
            _caller = caller;
            _eventConverter = eventConverter;
            _executionContext = executionContext;
            _callContextResolver = callContextResolver;
            _eventTypes = eventTypes;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates an <see cref="IEventStore"/> for the given tenant.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId">tenant</see> to create the event store for.</param>
        /// <returns>An <see cref="IEventStore"/>.</returns>
        public IEventStore ForTenant(TenantId tenantId)
            => new EventStore(
                CreateEventCommitter(tenantId),
                CreateAggregateEventCommitter(tenantId),
                CreateEventsForAggregateFetcher(tenantId));

        ICommitEvents CreateEventCommitter(TenantId tenantId)
            => new EventCommitter(
                    new Store.Internal.EventCommitter(
                        _caller,
                        _eventConverter,
                        _callContextResolver,
                        _executionContext.ForTenant(tenantId),
                        _loggerFactory.CreateLogger<EventCommitter>()),
                    _eventTypes);

        ICommitAggregateEvents CreateAggregateEventCommitter(TenantId tenantId)
            => new AggregateEventCommitter(
                    new Store.Internal.AggregateEventCommitter(
                        _caller,
                        _eventConverter,
                        _callContextResolver,
                        _executionContext.ForTenant(tenantId),
                        _loggerFactory.CreateLogger<Store.Internal.AggregateEventCommitter>()),
                    _eventTypes,
                    _loggerFactory.CreateLogger<AggregateEventCommitter>());

        IFetchEventsForAggregate CreateEventsForAggregateFetcher(TenantId tenantId)
            => new Store.Internal.EventsForAggregateFetcher(
                    _caller,
                    _eventConverter,
                    _callContextResolver,
                    _executionContext.ForTenant(tenantId),
                    _loggerFactory.CreateLogger<Store.Internal.EventsForAggregateFetcher>());
    }
}
