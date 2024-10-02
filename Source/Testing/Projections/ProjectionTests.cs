// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Testing.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Testing.Projections;

/// <summary>
/// Base class for testing projections.
/// </summary>
/// <typeparam name="TProjection"></typeparam>
public abstract class ProjectionTests<TProjection>
    where TProjection : ReadModel, new()
{
    EventLogSequenceNumber _sequenceNumber = EventLogSequenceNumber.Initial;
    readonly Dictionary<Key, TProjection> _projections = new();
    readonly IProjection<TProjection> _projection = ProjectionFixture<TProjection>.Projection;
    readonly ServiceProvider _serviceProvider;

    /// <summary>
    /// Gets the <see cref="IAggregates"/> for the test.
    /// This allows the test to perform actions on aggregates, and have the projections react to the events.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global - We want to expose this to the test
    protected IAggregates Aggregates { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionTests{TProjection}"/> class.
    /// </summary>
    /// <param name="configureServices">Allows any required services to be registered</param>
    protected ProjectionTests(Action<IServiceCollection>? configureServices = default)
    {
        var serviceCollection = new ServiceCollection();
        configureServices?.Invoke(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
        Aggregates = new AggregatesMock(_serviceProvider, OnAggregateEvents);
    }

    /// <summary>
    /// Perform an action on an aggregate. This will cause the aggregate to emit events, which will be picked up by the projection.
    /// This allows the test code to treat the events as a black box, and only focus on the aggregate and projection state.
    /// </summary>
    /// <param name="id">The aggregate ID</param>
    /// <param name="callback">The "Perform" statement</param>
    /// <typeparam name="TAggregate">The aggregate type</typeparam>
    protected void WhenAggregateMutated<TAggregate>(EventSourceId id, Action<TAggregate> callback) where TAggregate : AggregateRoot
    {
        Aggregates.Get<TAggregate>(id).Perform(callback, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Emit an event from an event source. This will cause the projection to react to the event.
    /// </summary>
    /// <param name="eventSource">The eventSourceId</param>
    /// <param name="evt">The event being produced</param>
    /// <param name="occurred">The timestamp of the event metadata</param>
    /// <typeparam name="TEvent">The type of the event</typeparam>
    /// <exception cref="ArgumentException"></exception>
    protected void WithEvent<TEvent>(EventSourceId eventSource, TEvent evt, DateTimeOffset? occurred = null) where TEvent : class
    {
        var eventType = EventTypeMetadata<TEvent>.EventType ?? throw new ArgumentException($"{typeof(TEvent)} is missing event type annotation");
        lock (this)
        {
            var sequenceNumber = _sequenceNumber++;
            var committedEvent = new CommittedEvent(
                sequenceNumber,
                occurred ?? DateTimeOffset.Now,
                eventSource,
                ExecutionContexts.Test,
                eventType,
                evt,
                false);
            On(committedEvent);
        }
    }

    /// <summary>
    /// Emit an event from an event source. This will cause the projection to react to the event.
    /// </summary>
    /// <param name="committedEvent"></param>
    protected void WithEvent(CommittedEvent committedEvent)
    {
        lock (this)
        {
            On(committedEvent);
        }
    }

    /// <summary>
    /// Assert against the state of the projection.
    /// </summary>
    protected ProjectionAssertions<TProjection> AssertThat => new(_projections);

    void OnAggregateEvents(UncommittedAggregateEvents events)
    {
        lock (this)
        {
            foreach (var evt in events)
            {
                var sequenceNumber = _sequenceNumber++;
                On(ToCommittedEvent(events, evt, sequenceNumber));
            }
        }
    }

    static CommittedEvent ToCommittedEvent(UncommittedAggregateEvents events, UncommittedAggregateEvent evt, EventLogSequenceNumber sequenceNumber)
    {
        return new CommittedEvent(
            sequenceNumber,
            DateTimeOffset.Now,
            events.EventSource,
            ExecutionContexts.Test,
            evt.EventType,
            evt.Content,
            evt.IsPublic
        );
    }

    void On(CommittedEvent evt)
    {
        var eventType = evt.EventType;
        if (!_projection.Events.TryGetValue(eventType, out var keySelector))
        {
            // not relevant to the projection
            return;
        }

        var context = ToEventContext(evt);
        var key = keySelector.GetKey(evt.Content, context);
        var existed = _projections.TryGetValue(key, out var projection);
        if (!existed)
        {
            var readModel = new TProjection
            {
                Id = key.Value,
            };
            // ReSharper disable once SuspiciousTypeConversion.Global
            if(readModel is IRequireDependencies<TProjection> requireDependencies)
            {
                requireDependencies.Resolve(_serviceProvider);
            }
            _projections[key] = projection = readModel;
            
        }

        var projectionContext = new ProjectionContext(!existed, key, context);
        var result = _projection.On(projection!, evt.Content, eventType, projectionContext);

        switch (result.Type)
        {
            case ProjectionResultType.Replace:
                var resultingModel = result.ReadModel;
                resultingModel!.SetLastUpdated(context.SequenceNumber,context.Occurred);
                _projections[key] = resultingModel;
                break;
            case ProjectionResultType.Delete:
                _projections.Remove(key);
                break;
        }
    }

    static EventContext ToEventContext(CommittedEvent evt)
    {
        return new EventContext(
            evt.EventLogSequenceNumber,
            evt.EventType,
            evt.EventSource,
            evt.Occurred,
            evt.ExecutionContext,
            ExecutionContexts.Test);
    }
}
