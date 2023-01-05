// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Represents a mock implementation of <see cref="IAggregateOf{TAggregateRoot}"/>.
/// </summary>
/// <typeparam name="TAggregate">The <see cref="Type"/> of the <see cref="AggregateRoot"/>.</typeparam>
public class AggregateOfMock<TAggregate> : IAggregateOf<TAggregate>
    where TAggregate : AggregateRoot
{
    readonly Func<EventSourceId, TAggregate> _createAggregateRoot;
    readonly ConcurrentDictionary<EventSourceId, object> _aggregateLocks = new();
    readonly ConcurrentDictionary<EventSourceId, TAggregate> _aggregates = new();
    readonly Dictionary<EventSourceId, List<AppliedEvent>> _eventsToApply = new();
    
    /// <summary>
    /// Initializes an instance of the <see cref="AggregateOfMock{T}"/> class.
    /// </summary>
    /// <param name="createAggregateRoot"></param>
    public AggregateOfMock(Func<EventSourceId, TAggregate> createAggregateRoot)
    {
        _createAggregateRoot = createAggregateRoot;
    }
    
    /// <summary>
    /// Gets all the aggregates that have had operations performed on them.
    /// </summary>
    public IEnumerable<TAggregate> Aggregates => _aggregates.Values;

    /// <inheritdoc />
    public IAggregateRootOperations<TAggregate> Get(EventSourceId eventSourceId)
    {
        var aggregate = GetOrAddAggregate(eventSourceId);
        var operations = new AggregateRootOperationsMock<TAggregate>(
            _aggregateLocks[eventSourceId],
            aggregate,
            () => _createAggregateRoot(eventSourceId),
            oldAggregate => _aggregates[eventSourceId] = oldAggregate);

        if (_eventsToApply.TryGetValue(eventSourceId, out var eventsToApply) && eventsToApply.Any())
        {
            operations.WithEvents(eventsToApply.ToArray());
            eventsToApply.Clear();
        }
        return operations;
    }

    /// <summary>
    /// Adds events that should be applied to the aggregate before an action is performed.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> to add the events for.</param>
    /// <param name="events">The events to apply.</param>
    /// <returns>The <see cref="AggregateOfMock{T}"/>.</returns>
    public AggregateOfMock<TAggregate> WithEventsFor(EventSourceId eventSource, params AppliedEvent[] events)
    {
        if (!_eventsToApply.TryGetValue(eventSource, out var appliedEvents))
        {
            appliedEvents = new List<AppliedEvent>();
            _eventsToApply[eventSource] = appliedEvents;
        }
        appliedEvents.AddRange(events);
        return this;
    }

    /// <summary>
    /// Adds events that should be applied to the aggregate before an action is performed
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> to add the events for.</param>
    /// <param name="events">The events to apply.</param>
    /// <returns>The <see cref="AggregateOfMock{T}"/>.</returns>
    public AggregateOfMock<TAggregate> WithEventsFor(EventSourceId eventSource, params object[] events)
        => WithEventsFor(eventSource, events.Select(_ => new AppliedEvent(_)));

    /// <summary>
    /// Tries to get the <typeparamref name="TAggregate"/> with the given <see cref="EventSourceId"/>.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> of the aggregate.</param>
    /// <param name="aggregate">The aggregate.</param>
    /// <returns>True if operations has been performed on aggregate, false if not.</returns>
    public bool TryGetAggregate(EventSourceId eventSource, out TAggregate aggregate)
        => _aggregates.TryGetValue(eventSource, out aggregate);

    /// <summary>
    /// Gets the aggregate with the given <see cref="EventSourceId"/>.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> of the aggregate</param>
    /// <returns>The <typeparamref name="TAggregate"/>.</returns>
    public TAggregate GetAggregate(EventSourceId eventSource)
        => GetOrAddAggregate(eventSource);

    TAggregate GetOrAddAggregate(EventSourceId eventSource)
        => _aggregates.GetOrAdd(eventSource, eventSourceId =>
        {
            _aggregateLocks.TryAdd(eventSourceId, new object());
            return _createAggregateRoot(eventSourceId);
        });
}

