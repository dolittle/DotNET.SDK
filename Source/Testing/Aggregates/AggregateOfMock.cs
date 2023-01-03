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

    /// <inheritdoc />
    public IAggregateRootOperations<TAggregate> Get(EventSourceId eventSourceId)
    {
        var aggregate = _aggregates.GetOrAdd(eventSourceId, eventSourceId =>
        {
            _aggregateLocks.TryAdd(eventSourceId, new object());
            return _createAggregateRoot(eventSourceId);
        });
         
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
    /// Adds events that should be applied to the aggregate before an action is performed
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
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
}
