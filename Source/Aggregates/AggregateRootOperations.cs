// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;

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
        bool _performed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootOperations{TAggregate}"/> class.
        /// </summary>
        /// <param name="eventStore">
        /// The <see cref="IEventStore" /> used for committing the <see cref="UncommittedAggregateEvents" />
        /// when actions are performed on the <typeparamref name="TAggregate">aggregate</typeparamref>.
        /// </param>
        /// <param name="aggregateRoot"><see cref="AggregateRoot"/> the operations are for.</param>
        public AggregateRootOperations(IEventStore eventStore, TAggregate aggregateRoot)
        {
            _aggregateRoot = aggregateRoot;
            _eventStore = eventStore;
        }

        /// <inheritdoc/>
        public Task Perform(Action<TAggregate> method)
        {
            var aggregateRootId = _aggregateRoot.GetAggregateRootId();
            if (_performed) throw new AggregateRootOperationAlreadyPerformed(typeof(TAggregate), aggregateRootId, _aggregateRoot.EventSourceId);
            _performed = true;
            method(_aggregateRoot);

            return _eventStore
                    .ForAggregate(aggregateRootId)
                    .WithEventSource(_aggregateRoot.EventSourceId)
                    .ExpectVersion(_aggregateRoot.Version)
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
