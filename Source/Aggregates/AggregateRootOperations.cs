// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateRootOperations{T}"/>.
    /// </summary>
    /// <typeparam name="TAggregate"><see cref="AggregateRoot"/> type.</typeparam>
    public class AggregateRootOperations<TAggregate> : IAggregateRootOperations<TAggregate>
        where TAggregate : AggregateRoot
    {
        readonly TAggregate _aggregateRoot;
        readonly IEventStore _eventStore;
        readonly ILogger _logger;
        bool _performed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootOperations{TAggregate}"/> class.
        /// </summary>
        /// <param name="eventStore">
        /// The <see cref="IEventStore" /> used for committing the <see cref="UncommittedAggregateEvents" />
        /// when actions are performed on the <typeparamref name="TAggregate">aggregate</typeparamref>.
        /// </param>
        /// <param name="aggregateRoot"><see cref="AggregateRoot"/> the operations are for.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateRootOperations(IEventStore eventStore, TAggregate aggregateRoot, ILogger logger)
        {
            _aggregateRoot = aggregateRoot;
            _eventStore = eventStore;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task Perform(Action<TAggregate> method)
            => Perform(aggregate =>
                {
                    method(aggregate);
                    return Task.CompletedTask;
                });

        /// <inheritdoc/>
        public async Task Perform(Func<TAggregate, Task> method)
        {
            var aggregateRootId = _aggregateRoot.GetAggregateRootId();
            _logger.LogDebug(
                "Performing operation on {AggregateRoot} with aggregate root id {AggregateRootId} applying events to event source {EventSource}",
                _aggregateRoot.GetType(),
                aggregateRootId,
                _aggregateRoot.EventSourceId);
            if (_performed) throw new AggregateRootOperationAlreadyPerformed(typeof(TAggregate), aggregateRootId, _aggregateRoot.EventSourceId);
            _performed = true;
            await method(_aggregateRoot).ConfigureAwait(false);
            if (_aggregateRoot.UncommittedEvents.Any()) await CommitAppliedEvents(aggregateRootId).ConfigureAwait(false);
        }

        Task<CommittedAggregateEvents> CommitAppliedEvents(AggregateRootId aggregateRootId)
        {
            _logger.LogDebug(
                "{AggregateRoot} with aggregate root id {AggregateRootId} is committing {NumberOfEvents} events to event source {EventSource}",
                _aggregateRoot.GetType(),
                aggregateRootId,
                _aggregateRoot.UncommittedEvents.Count(),
                _aggregateRoot.EventSourceId);
            return _eventStore
                    .ForAggregate(aggregateRootId)
                    .WithEventSource(_aggregateRoot.EventSourceId)
                    .ExpectVersion(_aggregateRoot.Version - (AggregateRootVersion)(uint)_aggregateRoot.UncommittedEvents.Count())
                    .Commit(CreateUncommittedEvents);
        }

        void CreateUncommittedEvents(UncommittedAggregateEventsBuilder builder)
        {
            foreach (var uncommittedEvent in _aggregateRoot.UncommittedEvents)
            {
                var eventBuilder = uncommittedEvent.IsPublic ?
                    builder.CreatePublicEvent(uncommittedEvent.Content)
                    : builder.CreateEvent(uncommittedEvent.Content);
                eventBuilder.WithEventType(uncommittedEvent.EventType);
            }
        }
    }
}
