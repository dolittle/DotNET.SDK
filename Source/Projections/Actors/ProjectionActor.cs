// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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

/// <summary>
/// Message to wait for an update to pass a specific sequence number.
/// </summary>
/// <param name="WaitForOffset"></param>
public record GetProjectionRequest(ulong WaitForOffset)
{
    public static readonly GetProjectionRequest GetCurrentValue = new(0);
}

public record SubscriptionRequest(PID Subscriber);

public record Unsubscribe(PID Subscriber);

public record Unsubscribed(Exception? Exception)
{
    public static readonly Unsubscribed Normally = new((Exception?)null);
}

public class ProjectionActor<TProjection>(
    GetServiceProviderForTenant getServiceProvider,
    IProjection<TProjection> projectionType,
    ILogger<ProjectionActor<TProjection>> logger,
    TimeSpan idleUnloadTimeout) : IActor where TProjection : ReadModel, new()
{
    Dictionary<ulong, TaskCompletionSource>? _waitingForUpdate;
    readonly TimeSpan _idleUnloadTimeout = idleUnloadTimeout > TimeSpan.Zero ? idleUnloadTimeout : TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// The cluster kind for the projection actor.
    /// </summary>
    public static string GetKind(IProjection<TProjection> projection) => $"proj_{projection.Identifier.Value:N}";

    IMongoCollection<TProjection>? _collection;
    string? _id;
    TProjection? _projection;
    bool _initialized;
    HashSet<PID>? _subscribers;

    public async Task ReceiveAsync(IContext context)
    {
        try
        {
            switch (context.Message)
            {
                case Started:
                    context.SetReceiveTimeout(_idleUnloadTimeout);
                    return;
                case ReceiveTimeout:
                    Unload(context);
                    return;
                case ProjectedEvent projectedEvent:
                    await Init(context);
                    await On(projectedEvent, context);
                    return;
                case GetProjectionRequest getRequest:
                    await Init(context);
                    OnGetProjectionRequest(getRequest, context);
                    return;
                case SubscriptionRequest request:
                    await Init(context);
                    OnSubscriptionRequest(request, context);
                    break;
                case Unsubscribe request:
                    OnUnsubscribe(request, context);
                    return;
                case DeadLetterResponse response:
                    RemoveSubscriber(context, response.Target);
                    return;

                default:
                    return;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error processing {Message}", context.Message);
            if (context.Sender != null)
            {
                context.Respond(new Try<ProjectionResultType>(e));
            }

            // ReSharper disable once MethodHasAsyncOverload
            Unload(context, e);
        }
    }

    void OnSubscriptionRequest(SubscriptionRequest request, IContext context)
    {
        if (context.Sender is null)
        {
            return;
        }

        RespondWithCurrentValue(context);
        _subscribers ??= new HashSet<PID>();
        _subscribers.Add(request.Subscriber);
        context.CancelReceiveTimeout(); // Keep the actor alive as long as there are subscribers
    }

    public void OnUnsubscribe(Unsubscribe request, IContext context)
    {
        RemoveSubscriber(context, request.Subscriber);
        context.Respond(Unsubscribed.Normally);
    }

    void RemoveSubscriber(IContext context, PID pid)
    {
        if (_subscribers is null)
        {
            return;
        }

        _subscribers.Remove(pid);
        if (_subscribers.Count == 0)
        {
            _subscribers = null;
            context.SetReceiveTimeout(_idleUnloadTimeout);
        }
    }


    void OnGetProjectionRequest(GetProjectionRequest getProjectionRequest, IContext context)
    {
        if (getProjectionRequest.WaitForOffset == 0ul || _projection?.EventOffset >= getProjectionRequest.WaitForOffset)
        {
            RespondWithCurrentValue(context);
        }
        else
        {
            OnWaitForUpdate(getProjectionRequest.WaitForOffset, context);
        }
    }


    void OnWaitForUpdate(ulong requestedOffset, IContext context)
    {
        // Already processed up to the requested sequence number. respond immediately
        if (_projection?.EventOffset >= requestedOffset)
        {
            RespondWithCurrentValue(context);
            return;
        }

        _waitingForUpdate ??= new Dictionary<ulong, TaskCompletionSource>();

        if (!_waitingForUpdate.TryGetValue(requestedOffset, out var tcs))
        {
            tcs = new TaskCompletionSource();
            _waitingForUpdate[requestedOffset] = tcs;
        }

        context.ReenterAfter(tcs.Task, () => RespondWithCurrentValue(context));
    }

    void RespondWithCurrentValue(IContext context)
    {
        context.Respond(new Try<TProjection?>(ToResponse(_projection)));
    }

    static TProjection? ToResponse(TProjection? current)
    {
        if (current is ICloneable cloneable)
        {
            return (TProjection?)cloneable.Clone();
        }

        return current;
    }

    void Unload(IContext context, Exception? exception = null)
    {
        if (_subscribers is not null)
        {
            var msg = exception is null ? Unsubscribed.Normally : new Unsubscribed(exception);
            foreach (var subscriber in _subscribers)
            {
                context.Send(subscriber, msg);
            }

            _subscribers = null;
        }

        context.Stop(context.Self);
    }

    async Task On(ProjectedEvent projectedEvent, IContext context)
    {
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
            if (projectedEvent.Context.SequenceNumber <= _projection!.EventOffset) // Event has already been processed
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
                _projection!.SetLastUpdated(projectionContext.EventContext.SequenceNumber.Value, projectionContext.EventContext.Occurred);
                OnReplace(_projection, context);
                await _collection!.ReplaceOneAsync(p => p.Id == _projection!.Id, _projection, new ReplaceOptions { IsUpsert = true });
                break;
            case ProjectionResultType.Delete:
                OnDeleted(context);
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

        TriggerWaitingForUpdate(projectedEvent.Context.SequenceNumber.Value);
    }

    void OnReplace(TProjection projection, IContext context)
    {
        if (_subscribers is null)
        {
            return;
        }

        var msg = new Try<TProjection?>(ToResponse(projection));
        foreach (var subscriber in _subscribers)
        {
            context.Send(subscriber, msg);
        }
    }

    void OnDeleted(IContext context)
    {
        if (_subscribers is null)
        {
            return;
        }

        var deleted = new Try<TProjection?>((TProjection?)null);
        foreach (var subscriber in _subscribers)
        {
            context.Send(subscriber, deleted);
            context.Send(subscriber, Unsubscribed.Normally);
        }
    }

    void TriggerWaitingForUpdate(ulong processedOffset)
    {
        if (_waitingForUpdate is null)
        {
            return;
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var key in _waitingForUpdate.Keys)
        {
            if (key > processedOffset) continue;
            if (_waitingForUpdate.Remove(key, out var tcs))
            {
                tcs.SetResult();
            }
        }

        if (_waitingForUpdate.Count == 0)
        {
            _waitingForUpdate = null;
        }
    }

    async ValueTask Init(IContext context)
    {
        if (_initialized)
        {
            return;
        }

        var id = context.ClusterIdentity();

        var (tenantId, key) = ClusterIdentityMapper.GetTenantAndKey(id);
        _id = key.Value;

        var sp = await getServiceProvider(tenantId);
        _collection = sp.GetRequiredService<IMongoCollection<TProjection>>();
        _projection = await _collection.Find(p => p.Id == _id).SingleOrDefaultAsync();
        _initialized = true;
    }
}
