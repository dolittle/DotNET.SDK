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
    /// Represents an implementation of <see cref="ICommitEvents" />.
    /// </summary>
    public class EventCommitter : ICommitEvents
    {
        static readonly EventStoreCommitMethod _commitMethod = new EventStoreCommitMethod();
        readonly IPerformMethodCalls _caller;
        readonly IEventTypes _eventTypes;
        readonly IEventConverter _eventConverter;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCommitter"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventCommitter(
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
        public Task<CommitEventsResult> Commit(
            object content,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default)
            => Commit(builder => BuildEvent(builder, content, eventSourceId), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(
            object content,
            EventSourceId eventSourceId,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => Commit(builder => BuildEvent(builder, content, eventSourceId, eventType: eventType), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> Commit(
            object content,
            EventSourceId eventSourceId,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => Commit(content, eventSourceId, new EventType(eventTypeId), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(
            object content,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default)
            => Commit(builder => BuildEvent(builder, content, eventSourceId, isPublic: true), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(
            object content,
            EventSourceId eventSourceId,
            EventType eventType,
            CancellationToken cancellationToken = default)
            => Commit(builder => BuildEvent(builder, content, eventSourceId, isPublic: true, eventType: eventType), cancellationToken);

        /// <inheritdoc/>
        public Task<CommitEventsResult> CommitPublic(
            object content,
            EventSourceId eventSourceId,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default)
            => CommitPublic(content, eventSourceId, new EventType(eventTypeId), cancellationToken);

        /// <inheritdoc/>
        public async Task<CommitEventsResult> Commit(
            Action<UncommittedEventsBuilder> callback,
            CancellationToken cancellationToken = default)
        {
            var builder = new UncommittedEventsBuilder();
            callback(builder);
            var uncommittedEvents = builder.Build(_eventTypes);
            _logger.LogDebug("Committing events");
            var request = new Contracts.CommitEventsRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
            };
            request.Events.AddRange(_eventConverter.ToProtobuf(uncommittedEvents));
            var response = await _caller.Call(_commitMethod, request, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response);
        }

        void BuildEvent(
            UncommittedEventsBuilder builder,
            object content,
            EventSourceId eventSourceId,
            bool isPublic = false,
            EventType eventType = default)
        {
            var uncommittedEventBuilder = isPublic ? builder.CreatePublicEvent(content) : builder.CreateEvent(content);
            var eventBuilder = uncommittedEventBuilder.FromEventSource(eventSourceId);
            if (eventType != default) eventBuilder.WithEventType(eventType);
        }
    }
}
