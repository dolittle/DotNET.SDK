// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommitAggregateEvents" />.
    /// </summary>
    public class AggregateEventCommitter : ICommitAggregateEvents
    {
        readonly Internal.ICommitAggregateEvents _aggregateEvents;
        readonly IEventTypes _eventTypes;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventCommitter"/> class.
        /// </summary>
        /// <param name="aggregateEvents">The <see cref="Internal.ICommitAggregateEvents" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateEventCommitter(
            Internal.ICommitAggregateEvents aggregateEvents,
            IEventTypes eventTypes,
            ILogger logger)
        {
            _aggregateEvents = aggregateEvents;
            _eventTypes = eventTypes;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default)
            => CommitWithBuilder(
                aggregateRootId,
                eventSourceId,
                expectedAggregateRootVersion,
                builder => BuildEvent(builder, content),
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => CommitWithBuilder(
                aggregateRootId,
                eventSourceId,
                expectedAggregateRootVersion,
                builder => BuildEvent(builder, content, eventType: eventType),
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => CommitForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                new EventType(eventTypeId),
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default)
            => CommitWithBuilder(
                aggregateRootId,
                eventSourceId,
                expectedAggregateRootVersion,
                builder => BuildEvent(builder, content, isPublic: true),
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => CommitWithBuilder(
                aggregateRootId,
                eventSourceId,
                expectedAggregateRootVersion,
                builder => BuildEvent(builder, content, isPublic: true, eventType: eventType),
                cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => CommitPublicForAggregate(
                content,
                eventSourceId,
                aggregateRootId,
                expectedAggregateRootVersion,
                new EventType(eventTypeId),
                cancellationToken);

        /// <inheritdoc/>
        public CommitForAggregateBuilder CommitForAggregate(AggregateRootId aggregateRootId, CancellationToken cancellationToken = default)
            => new CommitForAggregateBuilder(_aggregateEvents, _eventTypes, aggregateRootId, _logger);

        Task<CommitEventsForAggregateResult> CommitWithBuilder(
            AggregateRootId aggregateRootId,
            EventSourceId eventSourceId,
            AggregateRootVersion expectedVersion,
            Action<UncommittedAggregateEventsBuilder> callback,
            CancellationToken cancellationToken)
            => CommitForAggregate(aggregateRootId)
                .WithEventSource(eventSourceId)
                .ExpectVersion(expectedVersion)
                .Commit(callback, cancellationToken);

        void BuildEvent(UncommittedAggregateEventsBuilder builder, object content, bool isPublic = false, EventType eventType = default)
        {
            var eventBuilder = isPublic ? builder.CreatePublicEvent(content) : builder.CreateEvent(content);
            if (eventType != default) eventBuilder.WithEventType(eventType);
        }
    }
}
