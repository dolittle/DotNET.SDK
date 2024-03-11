// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Projections.Actors;

record ProjectedEvent(Key Key, object Event, EventType EventType, EventContext Context);

public class ProjectionActor<TProjection>(
    GetServiceProviderForTenant getServiceProvider,
    IProjection<TProjection> projectionType,
    ILogger<ProjectionActor<TProjection>> logger,
    TimeSpan idleUnloadTimeout) : IActor where TProjection : ReadModel, new()
{
    /// <summary>
    /// The cluster kind for the projection actor.
    /// </summary>
    public static string GetKind(IProjection<TProjection> projection) => $"proj_{projection.Identifier.Value:N}";

    IMongoCollection<TProjection>? _collection;
    string? _id;
    TProjection? _projection;
    bool _initialized;

    public async Task ReceiveAsync(IContext context)
    {
        try
        {
            switch (context.Message)
            {
                case Started:
                    await Init(context.ClusterIdentity()!, context);
                    return;
                case ReceiveTimeout:
                    // ReSharper disable once MethodHasAsyncOverload
                    context.Stop(context.Self);
                    return;
                case ProjectedEvent projectedEvent:
                    await On(projectedEvent, context);
                    return;
                default:
                    return;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error processing {Message}", context.Message);
            // ReSharper disable once MethodHasAsyncOverload
            context.Stop(context.Self);
        }
    }

    async Task On(ProjectedEvent projectedEvent, IContext context)
    {
        try
        {
            if (!_initialized)
            {
                await Init(context.ClusterIdentity()!, context);
            }

            var firstEvent = _projection is null;
            if (firstEvent)
            {
                _projection = new TProjection
                {
                    Id = _id!,
                };
            }
            else
            {
                if(projectedEvent.Context.SequenceNumber <= _projection!.EventOffset) // Event has already been processed
                {
                    context.Respond(new Try<ProjectionResultType>(ProjectionResultType.Keep));
                    return;
                }
            }

            var projectionContext = new ProjectionContext(firstEvent, projectedEvent.Key, projectedEvent.Context);
            var result = projectionType.On(_projection!, projectedEvent.Event, projectedEvent.EventType, projectionContext);
            switch (result.Type)
            {
                case ProjectionResultType.Replace:
                    _projection = result.ReadModel;
                    _projection!.SetLastUpdated(projectionContext.EventContext.SequenceNumber.Value,projectionContext.EventContext.Occurred);
                    await _collection!.ReplaceOneAsync(p => p.Id == _projection!.Id, _projection, new ReplaceOptions { IsUpsert = true });
                    break;
                case ProjectionResultType.Delete:
                    await _collection!.DeleteOneAsync(p => p.Id == _projection!.Id);
                    _projection = null;
                    // ReSharper disable once MethodHasAsyncOverload - Would deadlock the actor
                    context.Stop(context.Self);
                    break;
                case ProjectionResultType.Keep:
                default:
                    // No change
                    break;
            }
            context.Respond(new Try<ProjectionResultType>(result.Type));
        }
        catch (Exception e)
        {
            context.Respond(new Try<ProjectionResultType>(e));
        }
    }

    async Task Init(ClusterIdentity id, IContext context)
    {
        var (tenantId, key) = ClusterIdentityMapper.GetTenantAndKey(id);
        _id = key.Value;

        var sp = await getServiceProvider(tenantId);
        _collection = sp.GetRequiredService<IMongoCollection<TProjection>>();
        _projection = await _collection.Find(p => p.Id == _id).SingleOrDefaultAsync();
        context.SetReceiveTimeout(idleUnloadTimeout);
        _initialized = true;
    }
}
