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

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStore" />.
    /// </summary>
    public class EventStore : IEventStore
    {
        readonly IPerformMethodCalls _caller;
        readonly EventStoreCommitMethod _method = new EventStoreCommitMethod();
        readonly IEventConverter _eventConverter;
        readonly IExecutionContextManager _executionContextManager;
        readonly IEventTypes _eventTypes;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStore"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="executionContextManager">An <see cref="IExecutionContextManager"/> for getting execution context from.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventStore(
            IPerformMethodCalls caller,
            IEventConverter eventConverter,
            IExecutionContextManager executionContextManager,
            IEventTypes eventTypes,
            ILogger logger)
        {
            _caller = caller;
            _eventConverter = eventConverter;
            _executionContextManager = executionContextManager;
            _eventTypes = eventTypes;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CommittedEvents> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken)
        {
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response.Events);
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> Commit(UncommittedEvent uncommittedEvent, CancellationToken cancellationToken = default)
        {
            var uncommittedEvents = new UncommittedEvents();
            uncommittedEvents.Append(uncommittedEvent);
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response.Events[0]);
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> Commit(object content, EventSourceId eventSourceId, EventType eventType, CancellationToken cancellationToken = default)
        {
            var uncommittedEvents = ToUncommittedEvents(content, eventSourceId, eventType);
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response.Events[0]);
        }

        /// <inheritdoc/>
        public Task<CommittedEvent> Commit(object content, EventSourceId eventSourceId, CancellationToken cancellationToken = default)
        {
            var eventType = _eventTypes.GetFor(content.GetType());
            return Commit(content, eventSourceId, eventType, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> CommitPublic(object content, EventSourceId eventSourceId, EventType eventType, CancellationToken cancellationToken = default)
        {
            var uncommittedEvents = ToUncommittedEvents(content, eventSourceId, eventType, true);
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response.Events[0]);
        }

        /// <inheritdoc/>
        public Task<CommittedEvent> CommitPublic(object content, EventSourceId eventSourceId, CancellationToken cancellationToken = default)
        {
            var eventType = _eventTypes.GetFor(content.GetType());
            return CommitPublic(content, eventSourceId, eventType, cancellationToken);
        }

        async Task<Contracts.CommitEventsResponse> CommitInternal(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Committing events");
            var request = new Contracts.CommitEventsRequest
            {
                CallContext = GetCurrentCallContext(),
            };
            request.Events.AddRange(_eventConverter.ToProtobuf(uncommittedEvents));
            var response = await _caller.Call(_method, request, cancellationToken).ConfigureAwait(false);
            ThrowIfFailure(response.Failure);
            return response;
        }

        UncommittedEvents ToUncommittedEvents(object content, EventSourceId eventSourceId, EventType eventType, bool isPublic = false)
        {
            var uncommittedEvents = new UncommittedEvents();
            uncommittedEvents.Append(new UncommittedEvent(eventSourceId, eventType, content, isPublic));
            return uncommittedEvents;
        }

        CallRequestContext GetCurrentCallContext()
            => new CallRequestContext
            {
                // in the future this should be set to something more meaningfull
                HeadId = HeadId.NotSet.Value.ToProtobuf(),
                ExecutionContext = _executionContextManager.Current.ToProtobuf(),
            };

        void ThrowIfFailure(Failure failure)
        {
            if (failure != null) throw new EventStoreOperationFailed(failure.Reason);
        }
    }
}
