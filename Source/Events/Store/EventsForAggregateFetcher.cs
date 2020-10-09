// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Contracts = Dolittle.Runtime.Events.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsForAggregate" />.
    /// </summary>
    public class EventsForAggregateFetcher : IFetchEventsForAggregate
    {
        static readonly EventStoreFetchForAggregateMethod _fetchForAggregateMethod = new EventStoreFetchForAggregateMethod();
        readonly IPerformMethodCalls _caller;
        readonly IEventConverter _eventConverter;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsForAggregateFetcher"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsForAggregateFetcher(
            IPerformMethodCalls caller,
            IEventConverter eventConverter,
            IResolveCallContext callContextResolver,
            ExecutionContext executionContext,
            ILogger logger)
        {
            _caller = caller;
            _eventConverter = eventConverter;
            _callContextResolver = callContextResolver;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FetchForAggregateResult> FetchForAggregate(
            AggregateRootId aggregateRootId,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug(
                "Fetching events for aggregate root {AggregateRoot} and event source {EventSource}",
                aggregateRootId,
                eventSourceId);
            var request = new Contracts.FetchForAggregateRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Aggregate = new Contracts.Aggregate
                {
                    AggregateRootId = aggregateRootId.Value.ToProtobuf(),
                    EventSourceId = eventSourceId.Value.ToProtobuf()
                }
            };
            var response = await _caller.Call(_fetchForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response);
        }
    }
}
