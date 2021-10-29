// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
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
        readonly EventSourceId _eventSourceId;
        readonly IEventStore _eventStore;
        readonly ILogger _logger;
        readonly IEventTypes _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootOperations{TAggregate}"/> class.
        /// </summary>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the aggregate root instance.</param>
        /// <param name="eventStore">The <see cref="IEventStore" /> used for committing the <see cref="UncommittedAggregateEvents" /> when actions are performed on the <typeparamref name="TAggregate">aggregate</typeparamref>. </param>
        /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateRootOperations(EventSourceId eventSourceId, IEventStore eventStore, IEventTypes eventTypes, ILogger logger)
        {
            _eventSourceId = eventSourceId;
            _eventTypes = eventTypes;
            _eventStore = eventStore;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task Perform(Action<TAggregate> method, CancellationToken cancellationToken)
            => Perform(
                aggregate =>
                {
                    method(aggregate);
                    return Task.CompletedTask;
                },
                cancellationToken);

        /// <inheritdoc/>
        public async Task Perform(Func<TAggregate, Task> method, CancellationToken cancellationToken)
        {
            if (!TryGetAggregateRoot(_eventSourceId, out var aggregateRoot, out var exception))
            {
                throw new CouldNotGetAggregateRoot(typeof(TAggregate), _eventSourceId, exception.Message);
            }

            var aggregateRootId = aggregateRoot.GetAggregateRootId();

            await ReApplyEvents(aggregateRoot, aggregateRootId, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug(
                "Performing operation on {AggregateRoot} with aggregate root id {AggregateRootId} applying events to event source {EventSource}",
                aggregateRoot.GetType(),
                aggregateRootId,
                aggregateRoot.EventSourceId);
            await method(aggregateRoot).ConfigureAwait(false);

            if (aggregateRoot.AppliedEvents.Any()) await CommitAppliedEvents(aggregateRoot, aggregateRootId).ConfigureAwait(false);
        }

        bool TryGetAggregateRoot(EventSourceId eventSourceId, out TAggregate aggregateRoot, out Exception exception)
        {
            try
            {
                exception = default;
                _logger.LogDebug(
                    "Getting aggregate root {AggregateRoot} with event source id {EventSource}",
                    typeof(TAggregate),
                    eventSourceId);
                aggregateRoot = CreateAggregateRoot(eventSourceId);
                return true;
            }
            catch (Exception ex)
            {
                aggregateRoot = default;
                exception = ex;
                return false;
            }
        }

        TAggregate CreateAggregateRoot(EventSourceId eventSourceId)
        {
            var aggregateRootType = typeof(TAggregate);
            ThrowIfInvalidConstructor(aggregateRootType);
            var constructor = typeof(TAggregate).GetConstructors().Single();

            var aggregateRoot = GetInstanceFrom(eventSourceId, constructor);
            ThrowIfCouldNotCreateAggregateRoot(aggregateRoot);
            return aggregateRoot;
        }

        TAggregate GetInstanceFrom(EventSourceId id, ConstructorInfo constructor)
            => constructor.Invoke(new object[] { id }) as TAggregate;

        async Task ReApplyEvents(TAggregate aggregateRoot, AggregateRootId aggregateRootId, CancellationToken cancellationToken)
        {
            var eventSourceId = aggregateRoot.EventSourceId;
            _logger.LogDebug(
                "Re-applying events for {AggregateRoot} with aggregate root id {AggregateRootId} with event source id {EventSourceId}",
                typeof(TAggregate),
                aggregateRootId,
                eventSourceId);

            var committedEvents = await _eventStore.FetchForAggregate(aggregateRootId, eventSourceId, cancellationToken).ConfigureAwait(false);
            if (committedEvents.HasEvents)
            {
                _logger.LogTrace("Re-applying {NumberOfEvents} events", committedEvents.Count);
                aggregateRoot.ReApply(committedEvents);
            }
            else
            {
                _logger.LogTrace("No events to re-apply");
            }
        }

        Task<CommittedAggregateEvents> CommitAppliedEvents(TAggregate aggregateRoot, AggregateRootId aggregateRootId)
        {
            _logger.LogDebug(
                "{AggregateRoot} with aggregate root id {AggregateRootId} is committing {NumberOfEvents} events to event source {EventSource}",
                aggregateRoot.GetType(),
                aggregateRootId,
                aggregateRoot.AppliedEvents.Count(),
                aggregateRoot.EventSourceId);
            return _eventStore
                    .ForAggregate(aggregateRootId)
                    .WithEventSource(aggregateRoot.EventSourceId)
                    .ExpectVersion(aggregateRoot.Version.Value - (ulong)aggregateRoot.AppliedEvents.Count())
                    .Commit(builder => CreateUncommittedEvents(builder, aggregateRoot));
        }

        void CreateUncommittedEvents(UncommittedAggregateEventsBuilder builder, TAggregate aggregateRoot)
        {
            foreach (var appliedEvent in aggregateRoot.AppliedEvents)
            {
                var uncommittedEvent = ToUncommittedEvent(appliedEvent);
                var eventBuilder = uncommittedEvent.IsPublic ?
                    builder.CreatePublicEvent(uncommittedEvent.Content)
                    : builder.CreateEvent(uncommittedEvent.Content);
                eventBuilder.WithEventType(uncommittedEvent.EventType);
            }
        }

        UncommittedAggregateEvent ToUncommittedEvent(AppliedEvent appliedEvent)
        {
            var @event = appliedEvent.Event;
            var eventType = appliedEvent.EventType;
            if (appliedEvent.HasEventType) ThrowIfWrongEventType(@event, eventType);
            else eventType = _eventTypes.GetFor(@event.GetType());
            return new UncommittedAggregateEvent(eventType, @event, appliedEvent.Public);
        }

        void ThrowIfInvalidConstructor(Type type)
        {
            ThrowIfNotOneConstructor(type);
            ThrowIfConstructorIsInvalid(type, type.GetConstructors().Single());
        }

        void ThrowIfNotOneConstructor(Type type)
        {
            if (type.GetConstructors().Length != 1)
                throw new InvalidAggregateRootConstructorSignature(type, "expected only a single constructor");
        }

        void ThrowIfConstructorIsInvalid(Type type, ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            ThrowIfIncorrectParameter(type, parameters);
        }

        void ThrowIfIncorrectParameter(Type type, ParameterInfo[] parameters)
        {
            if (parameters.Length != 1 ||
                (parameters[0].ParameterType != typeof(Guid) &&
                 parameters[0].ParameterType != typeof(EventSourceId)))
            {
                throw new InvalidAggregateRootConstructorSignature(type, $"expected only one parameter and it must be of type {typeof(Guid)} or {typeof(EventSourceId)}");
            }
        }

        void ThrowIfCouldNotCreateAggregateRoot(TAggregate aggregateRoot)
        {
            if (aggregateRoot == default) throw new CouldNotCreateAggregateRootInstance(typeof(TAggregate));
        }

        void ThrowIfWrongEventType(object @event, EventType eventType)
        {
            var typeOfEvent = @event.GetType();
            if (_eventTypes.HasFor(typeOfEvent))
            {
                var associatedEventType = _eventTypes.GetFor(typeOfEvent);
                if (eventType != associatedEventType)
                    throw new ProvidedEventTypeDoesNotMatchEventTypeFromAttribute(eventType, associatedEventType, typeOfEvent);
            }
        }
    }
}
