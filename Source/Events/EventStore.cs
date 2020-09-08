// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Heads;
using Dolittle.Lifecycle;
using Dolittle.Protobuf;
using Dolittle.Services.Contracts;
using static Dolittle.Runtime.Events.Contracts.EventStore;
using Contracts = Dolittle.Runtime.Events.Contracts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStore" />.
    /// </summary>
    [SingletonPerTenant]
    public class EventStore : IEventStore
    {
        readonly EventStoreClient _eventStoreClient;
        readonly IArtifactTypeMap _artifactMap;
        readonly IEventConverter _eventConverter;
        readonly IExecutionContextManager _executionContextManager;
        readonly Head _head;
        readonly ILogger<EventStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStore"/> class.
        /// </summary>
        /// <param name="eventStoreClient">The event store grpc client.</param>
        /// <param name="artifactMap">The <see cref="IArtifactTypeMap" />.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="executionContextManager">An <see cref="IExecutionContextManager"/> for getting execution context from.</param>
        /// <param name="head">The current <see cref="Head"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventStore(
            EventStoreClient eventStoreClient,
            IArtifactTypeMap artifactMap,
            IEventConverter eventConverter,
            IExecutionContextManager executionContextManager,
            Head head,
            ILogger<EventStore> logger)
        {
            _artifactMap = artifactMap;
            _eventStoreClient = eventStoreClient;
            _eventConverter = eventConverter;
            _executionContextManager = executionContextManager;
            _head = head;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CommittedEvents> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Committing events");
            var request = new Contracts.CommitEventsRequest
            {
                CallContext = GetCurrentCallContext(),
            };
            request.Events.AddRange(_eventConverter.ToProtobuf(uncommittedEvents));
            var response = await _eventStoreClient.CommitAsync(request, cancellationToken: cancellationToken);
            ThrowIfFailure(response.Failure);
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
        public async Task<CommittedAggregateEvents> CommitForAggregate(UncommittedAggregateEvents uncommittedAggregateEvents, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Committing events for aggregate");
            var request = new Contracts.CommitAggregateEventsRequest
            {
                CallContext = GetCurrentCallContext(),
                Events = _eventConverter.ToProtobuf(uncommittedAggregateEvents),
            };
            var response = await _eventStoreClient.CommitForAggregateAsync(request, cancellationToken: cancellationToken);
            ThrowIfFailure(response.Failure);
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
        public async Task<CommittedAggregateEvents> FetchForAggregate(Type aggregateRoot, EventSourceId eventSource, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Fetching events for aggregate");
            var request = new Contracts.FetchForAggregateRequest
            {
                CallContext = GetCurrentCallContext(),
                Aggregate = new Contracts.Aggregate
                {
                    AggregateRootId = _artifactMap.GetArtifactFor(aggregateRoot).Id.ToProtobuf(),
                    EventSourceId = eventSource.ToProtobuf(),
                },
            };
            var response = await _eventStoreClient.FetchForAggregateAsync(request, cancellationToken: cancellationToken);
            ThrowIfFailure(response.Failure);
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
        public Task<CommittedAggregateEvents> FetchForAggregate<TAggregateRoot>(EventSourceId eventSource, CancellationToken cancellationToken)
            where TAggregateRoot : AggregateRoot
            => FetchForAggregate(typeof(TAggregateRoot), eventSource, cancellationToken);

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
