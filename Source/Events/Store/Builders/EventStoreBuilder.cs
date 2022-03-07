// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store.Builders;

/// <summary>
/// Represents an implementation of <see cref="IEventStoreBuilder"/>.
/// </summary>
public class EventStoreBuilder : IEventStoreBuilder
{
    readonly IPerformMethodCalls _caller;
    readonly IConvertEventsToProtobuf _eventToProtobufConverter;
    readonly IConvertEventsToSDK _eventToSDKConverter;
    readonly IConvertAggregateEventsToProtobuf _aggregateEventToProtobufConverter;
    readonly IConvertAggregateEventsToSDK _aggregateEventToSDKConverter;
    readonly ExecutionContext _executionContext;
    readonly IResolveCallContext _callContextResolver;
    readonly IEventTypes _eventTypes;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreBuilder"/> class.
    /// </summary>
    /// <param name="caller">The caller for unary calls.</param>
    /// <param name="eventToProtobufConverter">The <see cref="IConvertEventsToProtobuf" />.</param>
    /// <param name="eventToSDKConverter">The <see cref="IConvertEventsToSDK" />.</param>
    /// <param name="aggregateEventToProtobufConverter">The <see cref="IConvertAggregateEventsToProtobuf" />.</param>
    /// <param name="aggregateEventToSDKConverter">The <see cref="IConvertAggregateEventsToSDK" />.</param>
    /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
    /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public EventStoreBuilder(
        IPerformMethodCalls caller,
        IConvertEventsToProtobuf eventToProtobufConverter,
        IConvertEventsToSDK eventToSDKConverter,
        IConvertAggregateEventsToProtobuf aggregateEventToProtobufConverter,
        IConvertAggregateEventsToSDK aggregateEventToSDKConverter,
        ExecutionContext executionContext,
        IResolveCallContext callContextResolver,
        IEventTypes eventTypes,
        ILoggerFactory loggerFactory)
    {
        _caller = caller;
        _eventToProtobufConverter = eventToProtobufConverter;
        _eventToSDKConverter = eventToSDKConverter;
        _aggregateEventToProtobufConverter = aggregateEventToProtobufConverter;
        _aggregateEventToSDKConverter = aggregateEventToSDKConverter;
        _executionContext = executionContext;
        _callContextResolver = callContextResolver;
        _eventTypes = eventTypes;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IEventStore ForTenant(TenantId tenantId)
    {
        var executionContext = _executionContext
            .ForTenant(tenantId)
            .ForCorrelation(Guid.NewGuid());

        return new EventStore(
            CreateEventCommitter(executionContext),
            CreateAggregateEventCommitter(executionContext),
            CreateEventsForAggregateFetcher(executionContext));
    }

    ICommitEvents CreateEventCommitter(ExecutionContext executionContext)
        => new EventCommitter(
            new Internal.EventCommitter(
                _caller,
                _eventToProtobufConverter,
                _eventToSDKConverter,
                _callContextResolver,
                executionContext,
                _loggerFactory.CreateLogger<Internal.EventCommitter>()),
            _eventTypes);

    ICommitAggregateEvents CreateAggregateEventCommitter(ExecutionContext executionContext)
        => new AggregateEventCommitter(
            new Internal.AggregateEventCommitter(
                _caller,
                _aggregateEventToProtobufConverter,
                _aggregateEventToSDKConverter,
                _callContextResolver,
                executionContext,
                _loggerFactory.CreateLogger<Internal.AggregateEventCommitter>()),
            _eventTypes);

    IFetchEventsForAggregate CreateEventsForAggregateFetcher(ExecutionContext executionContext)
        => new Internal.EventsForAggregateFetcher(
            _caller,
            _aggregateEventToSDKConverter,
            _callContextResolver,
            executionContext,
            _loggerFactory.CreateLogger<Internal.EventsForAggregateFetcher>());
}
