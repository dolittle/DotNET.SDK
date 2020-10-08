// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using Contracts = Dolittle.Runtime.Events.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStore" />.
    /// </summary>
    public class EventStore : IEventStore
    {
        static readonly EventStoreCommitMethod _commitMethod = new EventStoreCommitMethod();
        static readonly EventStoreCommitForAggregateMethod _commitForAggregateMethod = new EventStoreCommitForAggregateMethod();
        static readonly EventStoreFetchForAggregateMethod _fetchForAggregateMethod = new EventStoreFetchForAggregateMethod();

        readonly IPerformMethodCalls _caller;
        readonly IEventConverter _eventConverter;
        readonly ExecutionContext _executionContext;
        readonly IEventTypes _eventTypes;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStore"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> to use.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventStore(
            IPerformMethodCalls caller,
            IEventConverter eventConverter,
            ExecutionContext executionContext,
            IEventTypes eventTypes,
            ILogger logger)
        {
            _caller = caller;
            _eventConverter = eventConverter;
            _executionContext = executionContext;
            _eventTypes = eventTypes;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CommitEventsResult> Commit(Action<UncommittedEventsBuilder> callback, CancellationToken cancellationToken = default)
        {
            var builder = new UncommittedEventsBuilder();
            callback(builder);
            var uncommittedEvents = builder.Build(_eventTypes);
            _logger.LogDebug("Committing events");
            var request = new Contracts.CommitEventsRequest
            {
                CallContext = GetCurrentCallContext(),
            };
            request.Events.AddRange(_eventConverter.ToProtobuf(uncommittedEvents));
            var response = await _caller.Call(_commitMethod, request, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response);
        }

        /// <inheritdoc/>
        public async Task<CommitEventsForAggregateResult> CommitForAggregate(
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedVersion,
            EventSourceId eventSourceId,
            Action<UncommittedAggregateEventsBuilder> callback,
            CancellationToken cancellationToken = default)
        {
            var builder = new UncommittedAggregateEventsBuilder(aggregateRootId, eventSourceId, expectedVersion);
            callback(builder);
            var uncommittedAggregateEvents = builder.Build(_eventTypes);
            _logger.LogDebug(
                "Committing events for aggregate root {AggregateRoot} with expected version {ExpectedVersion}",
                uncommittedAggregateEvents.AggregateRootId,
                uncommittedAggregateEvents.ExpectedAggregateRootVersion);

            var request = new Contracts.CommitAggregateEventsRequest
            {
                CallContext = GetCurrentCallContext(),
                Events = _eventConverter.ToProtobuf(uncommittedAggregateEvents)
            };
            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response);
        }

        /// <inheritdoc/>
        public async Task<FetchForAggregateResult> FetchForAggregate(AggregateRootId aggregateRootId, EventSourceId eventSourceId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug(
                "Fetching events for aggregate root {AggregateRoot} and event source {EventSource}",
                aggregateRootId,
                eventSourceId);
            var request = new Contracts.FetchForAggregateRequest
            {
                CallContext = GetCurrentCallContext(),
                Aggregate = new Contracts.Aggregate
                {
                    AggregateRootId = aggregateRootId.Value.ToProtobuf(),
                    EventSourceId = eventSourceId.Value.ToProtobuf()
                }
            };
            var response = await _caller.Call(_fetchForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response);
        }

        CallRequestContext GetCurrentCallContext()
            => new CallRequestContext
            {
                // In the future this should be set to something more meaningfull
                HeadId = HeadId.NotSet.Value.ToProtobuf(),
                ExecutionContext = _executionContext.ToProtobuf(),
            };
    }
}
