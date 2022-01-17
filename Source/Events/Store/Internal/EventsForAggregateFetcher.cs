// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    static readonly EventStoreFetchForAggregateMethod _fetchForAggregateMethod = new();
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
    public async Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
    {
        Log.FetchingEventsForAggregate(_logger, aggregateRootId, eventSourceId);
        var request = new Runtime.Events.Contracts.FetchForAggregateRequest
        {
            CallContext = _callContextResolver.ResolveFrom(_executionContext),
            Aggregate = new Runtime.Events.Contracts.Aggregate
            {
                AggregateRootId = aggregateRootId.Value.ToProtobuf(),
                EventSourceId = eventSourceId.Value,
            }
        };

        var response = await _caller.Call(_fetchForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
        response.Failure.ThrowIfFailureIsSet();

        if (_toSDK.TryConvert(response.Events, out var committedAggregateEvents, out var error))
        {
            return committedAggregateEvents;
        }
        Log.FetchedEventsForAggregateCouldNotBeConverted(_logger, error);
        throw error;

    }
}
