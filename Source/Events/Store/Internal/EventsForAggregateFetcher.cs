// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Diagnostics;
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
        var request = CreateFetchRequestBase(aggregateRootId, eventSourceId);
        request.FetchAllEvents = new FetchAllEventsForAggregateInBatchesRequest();
        return AggregateBatches(eventSourceId, aggregateRootId, DoFetchForAggregate(request, cancellationToken));
    }

    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes,
        CancellationToken cancellationToken = default)
    {
        _logger.FetchingEventsForAggregate(aggregateRootId, eventSourceId, eventTypes);
        var request = CreateFetchRequestBase(aggregateRootId, eventSourceId);
        request.FetchEvents = new FetchEventsForAggregateInBatchesRequest
        {
            EventTypes = { eventTypes.Select(_ => _.ToProtobuf()) }
        };
        return AggregateBatches(eventSourceId, aggregateRootId, DoFetchForAggregate(request, cancellationToken));
    }

    /// <inheritdoc />
    public IAsyncEnumerable<CommittedAggregateEvents> FetchStreamForAggregate(AggregateRootId aggregateRootId, EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
    {
        _logger.FetchingAllEventsForAggregate(aggregateRootId, eventSourceId);
        var request = CreateFetchRequestBase(aggregateRootId, eventSourceId);
        request.FetchAllEvents = new FetchAllEventsForAggregateInBatchesRequest();
        return DoFetchForAggregate(request, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<CommittedAggregateEvents> FetchStreamForAggregate(AggregateRootId aggregateRootId, EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes, CancellationToken cancellationToken = default)
    {
        _logger.FetchingEventsForAggregate(aggregateRootId, eventSourceId, eventTypes);
        var request = CreateFetchRequestBase(aggregateRootId, eventSourceId);
        request.FetchEvents = new FetchEventsForAggregateInBatchesRequest
        {
            EventTypes = { eventTypes.Select(_ => _.ToProtobuf()) }
        };
        return DoFetchForAggregate(request, cancellationToken);
    }

    async IAsyncEnumerable<CommittedAggregateEvents> DoFetchForAggregate(FetchForAggregateInBatchesRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in _caller.Call(_fetchForAggregateInBatchesMethod, request, cancellationToken))
        {
            response.Failure.ThrowIfFailureIsSet();
            if (!_toSDK.TryConvert(response.Events, out var batch, out var error))
            {
                _logger.FetchedEventsForAggregateCouldNotBeConverted(error);
                throw error;
            }

            yield return batch;
            var bytes = response.Events.Events.Sum(it => Encoding.UTF8.GetBytes(it.Content).Length);
            Metrics.EventsRehydrated(response.Events.Events.Count, bytes);
        }
    }

    static async Task<CommittedAggregateEvents> AggregateBatches(EventSourceId eventSourceId, AggregateRootId aggregateRootId,
        IAsyncEnumerable<CommittedAggregateEvents> batches)
    {
        var fetchedEvents =
            new CommittedAggregateEvents(eventSourceId, aggregateRootId, AggregateRootVersion.Initial, []);
        await foreach (var batch in batches)
        {
            fetchedEvents = new CommittedAggregateEvents(eventSourceId, aggregateRootId, batch.AggregateRootVersion, [.. fetchedEvents, .. batch]);
        }

        return fetchedEvents;
    }

    FetchForAggregateInBatchesRequest CreateFetchRequestBase(AggregateRootId aggregateRootId, EventSourceId eventSourceId)
        => new()
        {
            CallContext = _callContextResolver.ResolveFrom(_executionContext),
            Aggregate = new Aggregate
            {
                AggregateRootId = aggregateRootId.Value.ToProtobuf(),
                EventSourceId = eventSourceId.Value,
            },
        };
}
