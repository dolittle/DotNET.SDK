// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
#pragma warning disable CS0618 // Refers to EventSourceId which is marked obsolete for clients. Should still be used internally

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Stateful wrapper for an aggregate root instance.
/// </summary>
/// <typeparam name="TAggregate"></typeparam>
class AggregateWrapper<TAggregate> where TAggregate : AggregateRoot
{
    readonly EventSourceId _eventSourceId;
    readonly TenantId _tenant;
    readonly IEventStore _eventStore;
    readonly IEventTypes _eventTypes;
    readonly IServiceProvider _serviceProvider;
    readonly ILogger _logger;

    TAggregate? _instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateWrapper{TAggregate}"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the aggregate root instance.</param>
    /// <param name="tenant">The tenant id.</param>
    /// <param name="eventStore">The <see cref="IEventStore" /> used for committing the <see cref="UncommittedAggregateEvents" /> when actions are performed on the <typeparamref name="TAggregate">aggregate</typeparamref>. </param>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="serviceProvider">The tenant scoped <see cref="IServiceProvider"/>.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public AggregateWrapper(EventSourceId eventSourceId, TenantId tenant, IEventStore eventStore, IEventTypes eventTypes, IServiceProvider serviceProvider, ILogger<AggregateWrapper<TAggregate>> logger)
    {
        _eventSourceId = eventSourceId;
        _tenant = tenant;
        _eventTypes = eventTypes;
        _eventStore = eventStore;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Perform(Func<TAggregate, Task> method, CancellationToken cancellationToken = default)
    {
        using var activity = Tracing.ActivitySource.StartActivity($"{typeof(TAggregate).Name}.Perform")
            ?.Tag(_eventSourceId);

        try
        {
            _instance = await GetHydratedAggregate(cancellationToken);
            var aggregateRootId = _instance.AggregateRootId;
            activity?.Tag(aggregateRootId);
            _logger.PerformingOn(typeof(TAggregate), aggregateRootId, _instance.EventSourceId, _tenant);
            await method(_instance);
            if (_instance.AppliedEvents.Any())
            {
                await CommitAppliedEvents(_instance, aggregateRootId).ConfigureAwait(false);
                _instance.ClearAppliedEvents();
            }
        }
        catch (Exception e)
        {
            _instance = null; // Reset the instance so that it will be rehydrated next time
            activity?.RecordError(e);
            throw new AggregateRootOperationFailed(typeof(TAggregate), _eventSourceId, e);
        }
    }

    async ValueTask<TAggregate> GetHydratedAggregate(CancellationToken cancellationToken)
    {
        if (_instance is { } preHydratedAggregate) return preHydratedAggregate;

        if (!TryGetAggregateRoot(out var aggregateRoot, out var exception))
        {
            throw new CouldNotGetAggregateRoot(typeof(TAggregate), _eventSourceId, exception);
        }

        await Rehydrate(aggregateRoot, aggregateRoot.GetAggregateRootId(), cancellationToken);
        return aggregateRoot;
    }

    bool TryGetAggregateRoot(out TAggregate aggregateRoot, out Exception exception)
    {
        var getAggregateRoot = AggregateRootMetadata<TAggregate>.Construct(_serviceProvider, _eventSourceId);
        aggregateRoot = getAggregateRoot.Result;
        exception = getAggregateRoot.Exception;
        return getAggregateRoot.Success;
    }

    Task Rehydrate(TAggregate aggregateRoot, AggregateRootId aggregateRootId, CancellationToken cancellationToken)
    {
        var eventSourceId = aggregateRoot.EventSourceId;
        _logger.RehydratingAggregateRoot(typeof(TAggregate), aggregateRootId, eventSourceId, _tenant);
        var eventTypesToFetch = GetEventTypes(_eventTypes);
        var committedEventsBatches = _eventStore.FetchStreamForAggregate(aggregateRootId, eventSourceId, eventTypesToFetch, cancellationToken);
        return aggregateRoot.RehydrateInternal(committedEventsBatches, AggregateRootMetadata<TAggregate>.MethodsPerEventType, cancellationToken);
    }

    static bool IsStateLess => AggregateRootMetadata<TAggregate>.IsStateLess;

    /// <summary>
    /// Gets all the <see cref="IEnumerable{T}"/> of <see cref="EventType"/> that the aggregates handles
    /// </summary>
    /// <param name="eventTypes"></param>
    /// <returns></returns>
    static IEnumerable<EventType> GetEventTypes(IEventTypes eventTypes)
        => IsStateLess
            ? Enumerable.Empty<EventType>()
            :  AggregateRootMetadata<TAggregate>.MethodsPerEventType.Keys.Select(eventTypes.GetFor);

    Task<CommittedAggregateEvents> CommitAppliedEvents(TAggregate aggregateRoot, AggregateRootId aggregateRootId)
    {
        _logger.CommittingEvents(aggregateRoot.GetType(), aggregateRootId, aggregateRoot.AppliedEvents.Count(), aggregateRoot.EventSourceId, _tenant);
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
            var eventBuilder = uncommittedEvent.IsPublic
                ? builder.CreatePublicEvent(uncommittedEvent.Content)
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
            ThrowIfWrongEventType(@event, eventType!);
        }
        else
        {
            eventType = _eventTypes.GetFor(@event.GetType());
        }

        return new UncommittedAggregateEvent(eventType!, @event, appliedEvent.Public);
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
