// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store.Internal;

/// <summary>
/// Represents an implementation of <see cref="IFetchEventsForAggregate" />.
/// </summary>
public class EventsForAggregateFetcher : IFetchEventsForAggregate
{
    static readonly EventStoreFetchForAggregateInBatchesMethod _fetchForAggregateInBatchesMethod = new();
    
    readonly IPerformMethodCalls _caller;
    readonly IConvertAggregateEventsToSDK _toSDK;
    readonly IResolveCallContext _callContextResolver;
    readonly ExecutionContext _executionContext;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventsForAggregateFetcher"/> class.
    /// </summary>
    /// <param name="caller">The caller for unary calls.</param>
    /// <param name="toSDK">The <see cref="IConvertAggregateEventsToSDK" />.</param>
    /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public EventsForAggregateFetcher(
        IPerformMethodCalls caller,
        IConvertAggregateEventsToSDK toSDK,
        IResolveCallContext callContextResolver,
        ExecutionContext executionContext,
        ILogger logger)
    {
        _caller = caller;
        _toSDK = toSDK;
        _callContextResolver = callContextResolver;
        _executionContext = executionContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
    {
        _logger.FetchingAllEventsForAggregate(aggregateRootId, eventSourceId);
        return DoFetchForAggregate(aggregateRootId, eventSourceId, Enumerable.Empty<EventType>(), cancellationToken);
    }
    
    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes,
        CancellationToken cancellationToken = default)
    {
        _logger.FetchingEventsForAggregate(aggregateRootId, eventSourceId, eventTypes);
        return DoFetchForAggregate(aggregateRootId, eventSourceId, eventTypes, cancellationToken);
    }

    async Task<CommittedAggregateEvents> DoFetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes,
        CancellationToken cancellationToken)
    {
        var request = new Runtime.Events.Contracts.FetchForAggregateInBatchesRequest()
        {
            CallContext = _callContextResolver.ResolveFrom(_executionContext),
            Aggregate = new Runtime.Events.Contracts.Aggregate
            {
                AggregateRootId = aggregateRootId.Value.ToProtobuf(),
                EventSourceId = eventSourceId.Value,
            },
            EventTypes = { eventTypes.Select(_ => _.ToProtobuf()) }
        };

        var fetchedEvents = new CommittedAggregateEvents(eventSourceId, aggregateRootId, AggregateRootVersion.Initial, ImmutableList<CommittedAggregateEvent>.Empty);
        await foreach (var response in _caller.Call(_fetchForAggregateInBatchesMethod, request, cancellationToken))
        {
            response.Failure.ThrowIfFailureIsSet();
            if (!_toSDK.TryConvert(response.Events, out var batch, out var error))
            {
                _logger.FetchedEventsForAggregateCouldNotBeConverted(error);
                throw error;
            }
            
            fetchedEvents = new CommittedAggregateEvents(eventSourceId, aggregateRootId, batch.AggregateRootVersion, fetchedEvents.Concat(batch).ToList());
        }
        return fetchedEvents;
    }
}
