// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootOperations{T}"/>.
/// </summary>
/// <typeparam name="TAggregate"><see cref="AggregateRoot"/> type.</typeparam>
public class AggregateRootOperations<TAggregate> : IAggregateRootOperations<TAggregate>
    where TAggregate : AggregateRoot
{
    readonly EventSourceId _eventSourceId;
    readonly IEventStore _eventStore;
    readonly IEventTypes _eventTypes;
    readonly IAggregateRoots _aggregateRoots;
    readonly IServiceProvider _serviceProvider;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootOperations{TAggregate}"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the aggregate root instance.</param>
    /// <param name="eventStore">The <see cref="IEventStore" /> used for committing the <see cref="UncommittedAggregateEvents" /> when actions are performed on the <typeparamref name="TAggregate">aggregate</typeparamref>. </param>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/> used for getting an aggregate root instance.</param>
    /// <param name="serviceProvider">The tenant scoped <see cref="IServiceProvider"/>.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public AggregateRootOperations(EventSourceId eventSourceId, IEventStore eventStore, IEventTypes eventTypes, IAggregateRoots aggregateRoots, IServiceProvider serviceProvider, ILogger logger)
    {
        _eventSourceId = eventSourceId;
        _eventTypes = eventTypes;
        _eventStore = eventStore;
        _aggregateRoots = aggregateRoots;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task Perform(Action<TAggregate> method, CancellationToken cancellationToken = default)
        => Perform(
            aggregate =>
            {
                method(aggregate);
                return Task.CompletedTask;
            },
            cancellationToken);

    /// <inheritdoc/>
    public async Task Perform(Func<TAggregate, Task> method, CancellationToken cancellationToken = default)
    {
        using var activity = Tracing.ActivitySource.StartActivity($"{typeof(TAggregate).Name}.Perform")
            ?.Tag(_eventSourceId);

        try
        {
            if (!TryGetAggregateRoot(out var aggregateRoot, out var exception))
            {
                throw new CouldNotGetAggregateRoot(typeof(TAggregate), _eventSourceId, exception);
            }

            var aggregateRootId = aggregateRoot.GetAggregateRootId();
            activity?.Tag(aggregateRootId);
            await Rehydrate(aggregateRoot, aggregateRootId, cancellationToken).ConfigureAwait(false);
            _logger.PerformingOn(aggregateRoot.GetType(), aggregateRootId, aggregateRoot.EventSourceId);
            await method(aggregateRoot).ConfigureAwait(false);
            if (aggregateRoot.AppliedEvents.Any())
            {
                await CommitAppliedEvents(aggregateRoot, aggregateRootId).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            activity?.RecordError(e);
            throw new AggregateRootOperationFailed(typeof(TAggregate), _eventSourceId, e);
        }
    }

    bool TryGetAggregateRoot(out TAggregate aggregateRoot, out Exception exception)
    {
        var getAggregateRoot = _aggregateRoots.TryGet<TAggregate>(_eventSourceId, _serviceProvider);
        aggregateRoot = getAggregateRoot.Result;
        exception = getAggregateRoot.Exception;
        return getAggregateRoot.Success;
    }

    Task Rehydrate(TAggregate aggregateRoot, AggregateRootId aggregateRootId, CancellationToken cancellationToken)
    {
        var eventSourceId = aggregateRoot.EventSourceId;
        _logger.RehydratingAggregateRoot(typeof(TAggregate), aggregateRootId, eventSourceId);
        var eventTypesToFetch = aggregateRoot.GetEventTypes(_eventTypes);
        
        var committedEventsBatches = _eventStore.FetchStreamForAggregate(aggregateRootId, eventSourceId, eventTypesToFetch, cancellationToken);
        return aggregateRoot.Rehydrate(committedEventsBatches, cancellationToken);
    }

    Task<CommittedAggregateEvents> CommitAppliedEvents(TAggregate aggregateRoot, AggregateRootId aggregateRootId)
    {
        _logger.CommittingEvents(aggregateRoot.GetType(), aggregateRootId, aggregateRoot.AppliedEvents.Count(), aggregateRoot.EventSourceId);
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
        if (appliedEvent.HasEventType)
        {
            ThrowIfWrongEventType(@event, eventType);
        }
        else
        {
            eventType = _eventTypes.GetFor(@event.GetType());
        }

        return new UncommittedAggregateEvent(eventType, @event, appliedEvent.Public);
    }

    void ThrowIfWrongEventType(object @event, EventType eventType)
    {
        var typeOfEvent = @event.GetType();
        if (!_eventTypes.HasFor(typeOfEvent))
        {
            return;
        }

        var associatedEventType = _eventTypes.GetFor(typeOfEvent);
        if (eventType != associatedEventType)
        {
            throw new ProvidedEventTypeDoesNotMatchEventTypeFromAttribute(eventType, associatedEventType, typeOfEvent);
        }
    }
}
