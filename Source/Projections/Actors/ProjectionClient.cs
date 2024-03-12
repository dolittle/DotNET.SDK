// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Tenancy;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Projections.Actors;

public interface IProjectionClient<TProjection>
{
    /// <summary>
    /// Project an event to the projection.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="eventType"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProjectionResultType> On(object @event, EventType eventType, EventContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Get the current state of a projection.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellation"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<TProjection?> Get(string id, CancellationToken cancellation = default);

    /// <summary>
    /// Get the state of the projection after it has processed past a certain event offset.
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <param name="eventSequenceNumber"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task<TProjection?> GetAfter(string id, ulong eventSequenceNumber, CancellationToken cancellation = default);

    /// <summary>
    /// Get the current and later versions of the projection.
    /// Requires the projection to implement <see cref="ICloneable"/> in order to not return the same instance.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Subscription<TP?> Subscribe<TP>(string id, CancellationToken cancellationToken) where TP : TProjection, ICloneable;
}

public class ProjectionClient<TProjection>(IProjection<TProjection> projection, Cluster cluster, TenantId tenantId)
    : IProjectionClient<TProjection> where TProjection : ReadModel, new()
{
    readonly string _kind = ProjectionActor<TProjection>.GetKind(projection);

    public async Task<ProjectionResultType> On(object @event, EventType eventType, EventContext context, CancellationToken cancellationToken)
    {
        if (!projection.Events.TryGetValue(eventType, out var keySelector))
        {
            throw new UnhandledEventType($"Projection {projection.Identifier} does not handle event type {eventType}.", eventType);
        }

        var key = keySelector.GetKey(@event, context);
        var message = new ProjectedEvent(key, @event, eventType, context);

        var clusterIdentity = GetIdentity(context, key);
        var response = await cluster.RequestAsync<Try<ProjectionResultType>>(clusterIdentity, message, cancellationToken);
        response.ThrowIfFailed();

        return response.Result;
    }

    /// <inheritdoc />
    public async Task<TProjection?> Get(string id, CancellationToken cancellationToken = default)
    {
        var clusterIdentity = GetIdentity(tenantId, id);
        var response = await cluster.RequestAsync<Try<TProjection?>>(clusterIdentity, GetProjectionRequest.GetCurrentValue, cancellationToken);
        response.ThrowIfFailed();

        return response.Result;
    }

    /// <inheritdoc />
    public async Task<TProjection?> GetAfter(string id, ulong eventSequenceNumber, CancellationToken cancellationToken = default)
    {
        var clusterIdentity = GetIdentity(tenantId, id);
        var response = await cluster.RequestAsync<Try<TProjection?>>(
            clusterIdentity,
            new GetProjectionRequest(eventSequenceNumber),
            cancellationToken);
        response.ThrowIfFailed();

        return response.Result;
    }

    /// <inheritdoc />
    public Subscription<TCloneableProjection?> Subscribe<TCloneableProjection>(string id, CancellationToken cancellationToken)
        where TCloneableProjection : TProjection, ICloneable
    {
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var channel = Channel.CreateUnbounded<TCloneableProjection?>();

        cluster.System.Root.Spawn(Props.FromProducer(() =>
        {
            var projectionActorIdentity = GetIdentity(tenantId, id);
            return new SubscriptionActor<TCloneableProjection>(channel.Writer, projectionActorIdentity, linkedCts.Token);
        }));

        return new Subscription<TCloneableProjection?>(channel.Reader, linkedCts.Cancel);
    }

    ClusterIdentity GetIdentity(TenantId tenant, Key key) =>
        ClusterIdentityMapper.GetClusterIdentity(tenant, key, _kind);

    ClusterIdentity GetIdentity(EventContext context, Key key) =>
        ClusterIdentityMapper.GetClusterIdentity(context.CurrentExecutionContext.Tenant, key, _kind);
}

public class UnhandledEventType(string message, EventType eventType) : Exception(message)
{
    public EventType EventType => eventType;
}
