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

public abstract class ProjectionTests<TProjection>
    where TProjection : ReadModel, new()
{
    EventLogSequenceNumber _sequenceNumber = EventLogSequenceNumber.Initial;
    readonly Dictionary<Key, TProjection> _projections = new();
    readonly IProjection<TProjection> _projection = ProjectionFixture<TProjection>.Projection;
    protected IAggregates Aggregates { get; }

    protected void WhenAggregateMutated<TAggregate>(EventSourceId id, Action<TAggregate> callback) where TAggregate : AggregateRoot
    {
        Aggregates.Get<TAggregate>(id).Perform(callback, CancellationToken.None).GetAwaiter().GetResult();
    }

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

    protected void WithEvent(CommittedEvent committedEvent)
    {
        lock (this)
        {
            On(committedEvent);
        }
    }

    protected TProjection ReadModel(Key key)
    {
        return _projections.GetValueOrDefault(key) ?? throw new ReadModelDidNotExist(key);
    }

    protected void ReadModelShouldBeDeleted(Key key)
    {
        if (_projections.TryGetValue(key, out var projection))
        {
            throw new ReadModelExistedWhenItShouldNot(key, projection);
        }
    }

    protected ProjectionTests(Action<IServiceCollection>? configureServices = default)
    {
        var serviceCollection = new ServiceCollection();
        configureServices?.Invoke(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        Aggregates = new AggregatesMock(serviceProvider, OnAggregateEvents);
    }

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

    CommittedEvent ToCommittedEvent(UncommittedAggregateEvents events, UncommittedAggregateEvent evt, EventLogSequenceNumber sequenceNumber)
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
            _projections[key] = projection = new TProjection
            {
                Id = key.Value,
            };
        }

        var projectionContext = new ProjectionContext(!existed, key, context);
        var result = _projection.On(projection!, evt.Content, eventType, projectionContext);

        switch (result.Type)
        {
            case ProjectionResultType.Replace:
                var resultingModel = result.ReadModel;
                resultingModel!.SetLastUpdated(context.Occurred);
                _projections[key] = resultingModel;
                break;
            case ProjectionResultType.Delete:
                _projections.Remove(key);
                break;
        }
    }

    EventContext ToEventContext(CommittedEvent evt)
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

public class ReadModelExistedWhenItShouldNot : Exception
{
    public ReadModelExistedWhenItShouldNot(Key key, object projection)
        : base($"Read model for {key} existed when it should not. Projection: {projection}")
    {
    }
}

public class ReadModelDidNotExist : Exception
{
    public ReadModelDidNotExist(Key key)
        : base($"Read model for {key} did not exist")
    {
    }
}
