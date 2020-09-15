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
        readonly IEventTypes _eventTypes;
        readonly IEventConverter _eventConverter;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger<EventStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStore"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="evetTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="executionContextManager">An <see cref="IExecutionContextManager"/> for getting execution context from.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventStore(
            IPerformMethodCalls caller,
            IEventTypes evetTypes,
            IEventConverter eventConverter,
            IExecutionContextManager executionContextManager,
            ILogger<EventStore> logger)
        {
            _caller = caller;
            _eventTypes = evetTypes;
            _eventConverter = eventConverter;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CommittedEvents> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken)
        {
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            try
            {
                return _eventConverter.ToSDK(response.Events);
            }
            catch (CouldNotDeserializeEvent ex)
            {
                throw new CouldNotDeserializeEventFromScope(ScopeId.Default, ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> Commit(UncommittedEvent uncommittedEvent, CancellationToken cancellationToken = default)
        {
            var uncommittedEvents = new UncommittedEvents();
            uncommittedEvents.Append(uncommittedEvent);
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            try
            {
                return _eventConverter.ToSDK(response.Events[0]);
            }
            catch (CouldNotDeserializeEvent ex)
            {
                throw new CouldNotDeserializeEventFromScope(ScopeId.Default, ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> Commit(EventSourceId eventSourceId, EventType eventType, object content, CancellationToken cancellationToken = default)
        {
            var uncommittedEvents = new UncommittedEvents();
            uncommittedEvents.Append(new UncommittedEvent(eventSourceId, eventType, content, false));
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            try
            {
                return _eventConverter.ToSDK(response.Events[0]);
            }
            catch (CouldNotDeserializeEvent ex)
            {
                throw new CouldNotDeserializeEventFromScope(ScopeId.Default, ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> CommitPublic(EventSourceId eventSourceId, EventType eventType, object content, CancellationToken cancellationToken = default)
        {
            var uncommittedEvents = new UncommittedEvents();
            uncommittedEvents.Append(new UncommittedEvent(eventSourceId, eventType, content, false));
            var response = await CommitInternal(uncommittedEvents, cancellationToken).ConfigureAwait(false);
            try
            {
                return _eventConverter.ToSDK(response.Events[0]);
            }
            catch (CouldNotDeserializeEvent ex)
            {
                throw new CouldNotDeserializeEventFromScope(ScopeId.Default, ex);
            }
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

        CallRequestContext GetCurrentCallContext()
            => new CallRequestContext
            {
                HeadId = _head.Id.ToProtobuf(),
                ExecutionContext = _executionContextManager.Current.ToProtobuf(),
            };

        void ThrowIfFailure(Failure failure)
        {
            if (failure != null) throw new EventStoreOperationFailed(failure.Reason);
        }
    }
}
