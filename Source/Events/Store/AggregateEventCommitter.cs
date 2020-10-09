// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Contracts = Dolittle.Runtime.Events.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommitAggregateEvents" />.
    /// </summary>
    public class AggregateEventCommitter : ICommitAggregateEvents
    {
        static readonly EventStoreCommitForAggregateMethod _commitForAggregateMethod = new EventStoreCommitForAggregateMethod();
        readonly IPerformMethodCalls _caller;
        readonly IEventTypes _eventTypes;
        readonly IEventConverter _eventConverter;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventCommitter"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateEventCommitter(
            IPerformMethodCalls caller,
            IEventTypes eventTypes,
            IEventConverter eventConverter,
            IResolveCallContext callContextResolver,
            ExecutionContext executionContext,
            ILogger logger)
        {
            _caller = caller;
            _eventTypes = eventTypes;
            _eventConverter = eventConverter;
            _callContextResolver = callContextResolver;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default)
            => CommitForAggregate(
                eventSourceId,
                aggregateRootId,
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
            => CommitForAggregate(
                eventSourceId,
                aggregateRootId,
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
            => CommitForAggregate(
                eventSourceId,
                aggregateRootId,
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
            => CommitForAggregate(
                eventSourceId,
                aggregateRootId,
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
        public async Task<CommitEventsForAggregateResult> CommitForAggregate(
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedVersion,
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
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Events = _eventConverter.ToProtobuf(uncommittedAggregateEvents)
            };
            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response);
        }

        void BuildEvent(UncommittedAggregateEventsBuilder builder, object content, bool isPublic = false, EventType eventType = default)
        {
            var eventBuilder = isPublic ? builder.CreatePublicEvent(content) : builder.CreateEvent(content);
            if (eventType != default) eventBuilder.WithEventType(eventType);
        }
    }
}
