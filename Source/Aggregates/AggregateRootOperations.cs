// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.SDK.Aggregates.Actors;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootOperations{T}"/>.
/// </summary>
/// <typeparam name="TAggregate"><see cref="AggregateRoot"/> type.</typeparam>
public class AggregateRootOperations<TAggregate> : IAggregateRootOperations<TAggregate>
    where TAggregate : AggregateRoot
{
    readonly EventSourceId _eventSourceId;
    readonly IRootContext _context;
    readonly ClusterIdentity _clusterIdentity;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootOperations{TAggregate}"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the aggregate root instance.</param>
    /// <param name="eventStore">The <see cref="IEventStore" /> used for committing the <see cref="UncommittedAggregateEvents" /> when actions are performed on the <typeparamref name="TAggregate">aggregate</typeparamref>. </param>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/> used for getting an aggregate root instance.</param>
    /// <param name="serviceProvider">The tenant scoped <see cref="IServiceProvider"/>.</param>
    public AggregateRootOperations(IRootContext context, EventSourceId eventSourceId)
    {
        _context = context;
        _eventSourceId = eventSourceId;
        _clusterIdentity = GetClusterIdentity(_eventSourceId);
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
            await _context.System.Cluster()
                .RequestAsync<Try<bool>>(_clusterIdentity, new Perform<TAggregate>(method, cancellationToken), _context, cancellationToken);
        }
        catch (Exception e)
        {
            activity?.RecordError(e);
            throw new AggregateRootOperationFailed(typeof(TAggregate), _eventSourceId, e);
        }
    }

    static ClusterIdentity GetClusterIdentity(EventSourceId eventSourceId) =>
        ClusterIdentity.Create(eventSourceId, AggregateRootMetadata<TAggregate>.GetAggregateRootId().Value.ToString());
}
