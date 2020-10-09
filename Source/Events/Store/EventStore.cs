// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStore" />.
    /// </summary>
    public class EventStore : IEventStore
    {
        readonly ICommitEvents _events;
        readonly ICommitAggregateEvents _aggregateEvents;
        readonly IFetchEventsForAggregate _eventsForAggregate;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStore"/> class.
        /// </summary>
        /// <param name="events">The <see cref="ICommitEvents" />.</param>
        /// <param name="aggregateEvents">The <see cref="ICommitAggregateEvents" />.</param>
        /// <param name="eventsForAggregate">The <see cref="IFetchEventsForAggregate" />.</param>
        public EventStore(
            ICommitEvents events,
            ICommitAggregateEvents aggregateEvents,
            IFetchEventsForAggregate eventsForAggregate)
        {
            _events = events;
            _aggregateEvents = aggregateEvents;
            _eventsForAggregate = eventsForAggregate;
        }

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(
            object content,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default)
            => _events.Commit(content, eventSourceId, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(
            object content,
            EventSourceId eventSourceId,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => _events.Commit(content, eventSourceId, eventType, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(
            object content,
            EventSourceId eventSourceId,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => _events.Commit(content, eventSourceId, eventTypeId, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(
            Action<UncommittedEventsBuilder> callback,
            CancellationToken cancellationToken = default)
            => _events.Commit(callback, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedVersion,
            Action<UncommittedAggregateEventsBuilder> callback,
            CancellationToken cancellationToken = default)
            => _aggregateEvents.CommitForAggregate(
                eventSourceId,
                aggregateRootId,
                expectedVersion,
                callback,
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default)
            => _aggregateEvents.CommitForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => _aggregateEvents.CommitForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                eventType,
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => _aggregateEvents.CommitForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                eventTypeId,
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(
            object content,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default)
            => _events.CommitPublic(content, eventSourceId, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(
            object content,
            EventSourceId eventSourceId,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => _events.CommitPublic(content, eventSourceId, eventType, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(
            object content,
            EventSourceId eventSourceId,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => _events.CommitPublic(content, eventSourceId, eventTypeId, cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default)
            => _aggregateEvents.CommitPublicForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => _aggregateEvents.CommitPublicForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                eventType,
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => _aggregateEvents.CommitPublicForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                eventTypeId,
                cancellationToken);

        /// <inheritdoc/>
        public Task<FetchForAggregateResult> FetchForAggregate(
            AggregateRootId aggregateRootId,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default)
            => _eventsForAggregate.FetchForAggregate(aggregateRootId, eventSourceId, cancellationToken);
    }
}
