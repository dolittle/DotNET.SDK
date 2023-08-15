// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.SDK.Aggregates.Actors;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;
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
    /// <param name="tenantId">The <see cref="TenantId"/> of the current tenant.</param>
    /// <param name="context">The <see cref="IRootContext" />Root context used to communicate with actors</param>
    public AggregateRootOperations(EventSourceId eventSourceId, TenantId tenantId, IRootContext context)
    {
        _context = context;
        _eventSourceId = eventSourceId;
        _clusterIdentity = ClusterIdentityMapper.GetClusterIdentity<TAggregate>(tenantId, eventSourceId);
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
            var result = await _context.System.Cluster()
                .RequestAsync<Try<bool>>(_clusterIdentity, new Perform<TAggregate>(method, cancellationToken), _context,
                    cancellationToken);

            if (!result.Success)
            {
                throw result.Exception;
            }
        }
        catch (AggregateRootOperationFailed e) when(e.InnerException is not null)
        {
            activity?.RecordError(e.InnerException);
            // ReSharper disable once PossibleIntendedRethrow
#pragma warning disable CA2200
            // Here we would like the stacktrace to be updated with this stack instead of the actor stack
            throw e;
#pragma warning restore CA2200
        }
        catch (Exception e)
        {
            activity?.RecordError(e);
            throw new AggregateRootOperationFailed(typeof(TAggregate), _eventSourceId, e);
        }
    }
}
