// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Initializes a new instance 
/// </summary>
/// <typeparam name="TAggregate"></typeparam>
public class AggregateRootOperationsMock<TAggregate> : IAggregateRootOperations<TAggregate>
    where TAggregate : AggregateRoot
{
    readonly object _concurrencyLock;
    readonly TAggregate _aggregateRoot;
    readonly Func<TAggregate> _createAggregate;
    readonly Action<TAggregate> _persistOldAggregate;
    readonly List<AppliedEvent> _eventsToApply = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootOperationsMock{TAggregate}"/> class.
    /// </summary>
    /// <param name="concurrencyLock">The lock object used for accessing the aggregate without conflicting with other concurrent operations.</param>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot"/>.</param>
    /// <param name="createAggregate">The callback for creating a new clean aggregate.</param>
    /// <param name="persistOldAggregate"></param>
    public AggregateRootOperationsMock(
        object concurrencyLock,
        TAggregate aggregateRoot,
        Func<TAggregate> createAggregate,
        Action<TAggregate> persistOldAggregate)
    {
        _concurrencyLock = concurrencyLock;
        _aggregateRoot = aggregateRoot;
        _createAggregate = createAggregate;
        _persistOldAggregate = persistOldAggregate;
    }

    /// <inheritdoc />
    public Task Perform(Action<TAggregate> method, CancellationToken cancellationToken = default)
        => Perform(_ =>
        {
            method(_);
            return Task.CompletedTask;
        }, cancellationToken);

    /// <inheritdoc />
    public Task Perform(Func<TAggregate, Task> method, CancellationToken cancellationToken = default)
    {
        lock (_concurrencyLock)
        {
            var previousAppliedEvents = new ReadOnlyCollection<AppliedEvent>(_aggregateRoot.AppliedEvents.ToList());
            try
            {
                ApplyEvents(_aggregateRoot, _eventsToApply);
                _eventsToApply.Clear();
                method(_aggregateRoot).GetAwaiter().GetResult();
                return Task.CompletedTask;
            }
            catch
            {
                var oldAggregate = _createAggregate();
                ApplyEvents(oldAggregate, previousAppliedEvents);
                _persistOldAggregate(oldAggregate);
                throw;
            }
        }
    }

    /// <summary>
    /// Adds events that should be applied to the aggregate before an action is performed
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    public AggregateRootOperationsMock<TAggregate> WithEvents(params AppliedEvent[] events)
    {
        _eventsToApply.AddRange(events);
        return this;
    }

    static void ApplyEvents(TAggregate aggregate, IEnumerable<AppliedEvent> events)
    {
        foreach (var e in events)
        {
            if (e.Public)
            {
                aggregate.ApplyPublic(e.Event, e.EventType!);
            }
            else
            {
                aggregate.Apply(e.Event, e.EventType!);
            }
        }
    }
}
