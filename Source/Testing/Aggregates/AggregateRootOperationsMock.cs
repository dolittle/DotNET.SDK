// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Initializes a new instance 
/// </summary>
/// <typeparam name="TAggregate"></typeparam>
public class AggregateRootOperationsMock<TAggregate> : IAggregateRootOperations<TAggregate>
    where TAggregate : AggregateRoot
{
    readonly EventSourceId _eventSourceId;
    readonly object _concurrencyLock;
    readonly TAggregate _aggregateRoot;
    readonly Func<TAggregate> _createAggregate;
    readonly Action<TAggregate> _persistOldAggregate;
    readonly Action<int> _persistNumEventsBeforeLastOperation;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootOperationsMock{TAggregate}"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/>.</param>
    /// <param name="concurrencyLock">The lock object used for accessing the aggregate without conflicting with other concurrent operations.</param>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot"/>.</param>
    /// <param name="createAggregate">The callback for creating a new clean aggregate.</param>
    /// <param name="persistOldAggregate"></param>
    /// <param name="persistNumEventsBeforeLastOperation"></param>
    public AggregateRootOperationsMock(
        EventSourceId eventSourceId,
        object concurrencyLock,
        TAggregate aggregateRoot,
        Func<TAggregate> createAggregate,
        Action<TAggregate> persistOldAggregate,
        Action<int> persistNumEventsBeforeLastOperation)
    {
        _eventSourceId = eventSourceId;
        _concurrencyLock = concurrencyLock;
        _aggregateRoot = aggregateRoot;
        _createAggregate = createAggregate;
        _persistOldAggregate = persistOldAggregate;
        _persistNumEventsBeforeLastOperation = persistNumEventsBeforeLastOperation;
    }

    /// <inheritdoc />
    public Task Perform(Action<TAggregate> method, CancellationToken cancellationToken = default)
        => Perform(_ =>
        {
            method(_);
            return Task.CompletedTask;
        }, cancellationToken);

    /// <summary>
    /// Performs operation on aggregate synchronously.
    /// </summary>
    /// <param name="method">The method to perform.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public void PerformSync(Action<TAggregate> method, CancellationToken cancellationToken = default)
        => Perform(_ =>
        {
            method(_);
            return Task.CompletedTask;
        }, cancellationToken).GetAwaiter().GetResult();
    
    /// <summary>
    /// Performs operation on aggregate synchronously.
    /// </summary>
    /// <param name="method">The method to perform.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public void PerformSync(Func<TAggregate, Task> method, CancellationToken cancellationToken = default)
        => Perform(method, cancellationToken).GetAwaiter().GetResult();
    
    /// <inheritdoc />
    public Task Perform(Func<TAggregate, Task> method, CancellationToken cancellationToken = default)
    {
        lock (_concurrencyLock)
        {
            var previousAppliedEvents = new ReadOnlyCollection<AppliedEvent>(_aggregateRoot.AppliedEvents.ToList());
            try
            {
                _persistNumEventsBeforeLastOperation(previousAppliedEvents.Count);
                method(_aggregateRoot).GetAwaiter().GetResult();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                var oldAggregate = _createAggregate();
                ApplyEvents(oldAggregate, previousAppliedEvents);
                _persistOldAggregate(oldAggregate);
                throw new AggregateRootOperationFailed(typeof(TAggregate), _eventSourceId, ex);
            }
        }
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
