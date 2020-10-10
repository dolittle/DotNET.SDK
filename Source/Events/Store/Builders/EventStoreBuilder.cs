// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store.Builders
{
    /// <summary>
    /// Represents a builder for building <see cref="EventStore"/>.
    /// </summary>
    public class EventStoreBuilder
    {
        readonly IPerformMethodCalls _caller;
        readonly IConvertEventsToProtobuf _eventToProtobufConverter;
        readonly IConvertEventResponsesToSDK _eventResponsesToSDKConverter;
        readonly IConvertAggregateEventsToProtobuf _aggregateEventToProtobufConverter;
        readonly IConvertAggregateResponsesToSDK _aggregateResponsesToSDKConverter;
        readonly ExecutionContext _executionContext;
        readonly IResolveCallContext _callContextResolver;
        readonly IEventTypes _eventTypes;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreBuilder"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventToProtobufConverter">The <see cref="IConvertEventsToProtobuf" />.</param>
        /// <param name="eventResponsesToSDKConverter">The <see cref="IConvertEventResponsesToSDK" />.</param>
        /// <param name="aggregateEventToProtobufConverter">The <see cref="IConvertAggregateEventsToProtobuf" />.</param>
        /// <param name="aggregateResponsesToSDKConverter">The <see cref="IConvertAggregateResponsesToSDK" />.</param>
        /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventStoreBuilder(
            IPerformMethodCalls caller,
            IConvertEventsToProtobuf eventToProtobufConverter,
            IConvertEventResponsesToSDK eventResponsesToSDKConverter,
            IConvertAggregateEventsToProtobuf aggregateEventToProtobufConverter,
            IConvertAggregateResponsesToSDK aggregateResponsesToSDKConverter,
            ExecutionContext executionContext,
            IResolveCallContext callContextResolver,
            IEventTypes eventTypes,
            ILoggerFactory loggerFactory)
        {
            _caller = caller;
            _eventToProtobufConverter = eventToProtobufConverter;
            _eventResponsesToSDKConverter = eventResponsesToSDKConverter;
            _aggregateEventToProtobufConverter = aggregateEventToProtobufConverter;
            _aggregateResponsesToSDKConverter = aggregateResponsesToSDKConverter;
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
                    new Internal.EventCommitter(
                        _caller,
                        _eventToProtobufConverter,
                        _eventResponsesToSDKConverter,
                        _callContextResolver,
                        _executionContext.ForTenant(tenantId),
                        _loggerFactory.CreateLogger<Internal.EventCommitter>()),
                    _eventTypes);

        ICommitAggregateEvents CreateAggregateEventCommitter(TenantId tenantId)
            => new AggregateEventCommitter(
                    new Internal.AggregateEventCommitter(
                        _caller,
                        _aggregateEventToProtobufConverter,
                        _aggregateResponsesToSDKConverter,
                        _callContextResolver,
                        _executionContext.ForTenant(tenantId),
                        _loggerFactory.CreateLogger<Internal.AggregateEventCommitter>()),
                    _eventTypes,
                    _loggerFactory.CreateLogger<AggregateEventCommitter>());

        IFetchEventsForAggregate CreateEventsForAggregateFetcher(TenantId tenantId)
            => new Internal.EventsForAggregateFetcher(
                    _caller,
                    _aggregateResponsesToSDKConverter,
                    _callContextResolver,
                    _executionContext.ForTenant(tenantId),
                    _loggerFactory.CreateLogger<Internal.EventsForAggregateFetcher>());
    }
}