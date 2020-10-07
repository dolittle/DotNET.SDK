// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
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
        public Task<CommitEventsResult> Commit(object content, EventSourceId eventSourceId, CancellationToken cancellationToken = default)
            => Commit(content, eventSourceId, _eventTypes.GetFor(content.GetType()), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(object content, EventSourceId eventSourceId, EventType eventType, CancellationToken cancellationToken = default)
            => Commit(ToUncommittedEvent(content, eventSourceId, eventType, false), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(object content, EventSourceId eventSourceId, EventTypeId eventTypeId, CancellationToken cancellationToken = default)
            => Commit(content, eventSourceId, new EventType(eventTypeId), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(object content, EventSourceId eventSourceId, CancellationToken cancellationToken = default)
            => CommitPublic(content, eventSourceId, _eventTypes.GetFor(content.GetType()), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(object content, EventSourceId eventSourceId, EventType eventType, CancellationToken cancellationToken = default)
            => Commit(ToUncommittedEvent(content, eventSourceId, eventType, true), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(object content, EventSourceId eventSourceId, EventTypeId eventTypeId, CancellationToken cancellationToken = default)
            => CommitPublic(content, eventSourceId, new EventType(eventTypeId), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(UncommittedEvent uncommittedEvent, CancellationToken cancellationToken = default)
            => Commit(new UncommittedEvents { uncommittedEvent }, cancellationToken);

        /// <inheritdoc/>
        public async Task<CommitEventsResult> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken)
        {
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
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

        /// <inheritdoc/>
        public async Task<CommitEventsForAggregateResult> CommitForAggregate(UncommittedAggregateEvents uncommittedAggregateEvents, CancellationToken cancellationToken = default)
        {
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
        public Task<CommitEventsForAggregateResult> CommitForAggregate(object content, EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion, CancellationToken cancellationToken = default)
            => CommitForAggregate(content, eventSourceId, aggregateRootId, expectedAggregateRootVersion, _eventTypes.GetFor(content.GetType()), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(object content, EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion, EventType eventType, CancellationToken cancellationToken = default)
            => CommitForAggregate(ToUncommittedAggregateEvent(content, eventType, false), eventSourceId, aggregateRootId, expectedAggregateRootVersion, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(object content, EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion, EventTypeId eventTypeId, CancellationToken cancellationToken = default)
            => CommitForAggregate(content, eventSourceId, aggregateRootId, expectedAggregateRootVersion, new EventType(eventTypeId), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(object content, EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion, CancellationToken cancellationToken = default)
            => CommitPublicForAggregate(content, eventSourceId, aggregateRootId, expectedAggregateRootVersion, _eventTypes.GetFor(content.GetType()), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(object content, EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion, EventType eventType, CancellationToken cancellationToken = default)
            => CommitForAggregate(ToUncommittedAggregateEvent(content, eventType, true), eventSourceId, aggregateRootId, expectedAggregateRootVersion, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(object content, EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion, EventTypeId eventTypeId, CancellationToken cancellationToken = default)
            => CommitPublicForAggregate(content, eventSourceId, aggregateRootId, expectedAggregateRootVersion, new EventType(eventTypeId), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(UncommittedAggregateEvent uncommittedAggregateEvent, EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion, CancellationToken cancellationToken = default)
            => CommitForAggregate(
                new UncommittedAggregateEvents(eventSourceId, aggregateRootId, expectedAggregateRootVersion)
                {
                    uncommittedAggregateEvent
                },
                cancellationToken);

        Task<Contracts.CommitEventsResponse> CommitInternal(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Committing events");
            var request = new Contracts.CommitEventsRequest
            {
                CallContext = GetCurrentCallContext(),
            };
            request.Events.AddRange(_eventConverter.ToProtobuf(uncommittedEvents));
            return _caller.Call(_commitMethod, request, cancellationToken);
        }

        UncommittedEvent ToUncommittedEvent(object content, EventSourceId eventSourceId, EventType eventType, bool isPublic)
            => new UncommittedEvent(eventSourceId, eventType, content, isPublic);

        UncommittedAggregateEvent ToUncommittedAggregateEvent(object content, EventType eventType, bool isPublic)
            => new UncommittedAggregateEvent(eventType, content, isPublic);

        CallRequestContext GetCurrentCallContext()
            => new CallRequestContext
            {
                // in the future this should be set to something more meaningfull
                HeadId = HeadId.NotSet.Value.ToProtobuf(),
                ExecutionContext = _executionContext.ToProtobuf(),
            };
    }
}
