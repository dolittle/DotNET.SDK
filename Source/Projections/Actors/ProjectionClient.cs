// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
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
}

public class ProjectionClient<TProjection>(IProjection<TProjection> projection, Cluster cluster) : IProjectionClient<TProjection> where TProjection : ProjectionBase, new()
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

    ClusterIdentity GetIdentity(EventContext context, Key key) =>
        ClusterIdentityMapper.GetClusterIdentity(context.CurrentExecutionContext.Tenant, key, _kind);
}

public class UnhandledEventType(string message, EventType eventType) : Exception(message)
{
    public EventType EventType => eventType;
}
