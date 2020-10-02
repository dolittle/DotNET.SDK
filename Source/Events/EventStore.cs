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
        static readonly EventStoreCommitMethod _method = new EventStoreCommitMethod();

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

        Task<Contracts.CommitEventsResponse> CommitInternal(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Committing events");
            var request = new Contracts.CommitEventsRequest
            {
                CallContext = GetCurrentCallContext(),
            };
            request.Events.AddRange(_eventConverter.ToProtobuf(uncommittedEvents));
            return _caller.Call(_method, request, cancellationToken);
        }

        UncommittedEvent ToUncommittedEvent(object content, EventSourceId eventSourceId, EventType eventType, bool isPublic)
            => new UncommittedEvent(eventSourceId, eventType, content, isPublic);

        CallRequestContext GetCurrentCallContext()
            => new CallRequestContext
            {
                // in the future this should be set to something more meaningfull
                HeadId = HeadId.NotSet.Value.ToProtobuf(),
                ExecutionContext = _executionContext.ToProtobuf(),
            };
    }
}
