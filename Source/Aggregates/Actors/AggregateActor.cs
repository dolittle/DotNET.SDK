// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Aggregates.Actors;


delegate TimeSpan AggregateUnloadTimeout();

class Perform<TAggregate>(Func<TAggregate, Task> callback, CancellationToken cancellationToken) where TAggregate : AggregateRoot
{
    public Func<TAggregate, Task> Callback { get; } = callback;
    public CancellationToken CancellationToken { get; } = cancellationToken;
}

class PerformAndRespond<TAggregate>(Func<TAggregate, object?> callback, CancellationToken cancellationToken) where TAggregate : AggregateRoot
{
    public Func<TAggregate, object?> Callback { get; } = callback;
    public CancellationToken CancellationToken { get; } = cancellationToken;
}

class AggregateActor<TAggregate> : IActor where TAggregate : AggregateRoot
{
    readonly GetServiceProviderForTenant _getServiceProvider;
    readonly ILogger<AggregateActor<TAggregate>> _logger;
    AggregateWrapper<TAggregate>? _aggregateWrapper;

    EventSourceId? _eventSourceId;

    readonly TimeSpan _idleUnloadTimeout;

    internal AggregateActor(GetServiceProviderForTenant getServiceProvider, ILogger<AggregateActor<TAggregate>> logger, TimeSpan idleUnloadTimeout)
    {
        _getServiceProvider = getServiceProvider;
        _logger = logger;
        _idleUnloadTimeout = idleUnloadTimeout;
    }

    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            Started => OnStarted(context),
            Stopping => OnStopping(context),
            ReceiveTimeout => OnReceiveTimeout(context),
            Perform<TAggregate> msg => OnPerform(msg, context),
            PerformAndRespond<TAggregate> msg => OnPerformAndRespond(msg, context),
            _ => Task.CompletedTask
        };
    }

    Task OnStopping(IContext _)
    {
        _logger.UnloadingAggregate(typeof(TAggregate));
        return Task.CompletedTask;
    }

    static Task OnReceiveTimeout(IContext context)
    {
        context.Poison(context.Self);
        return Task.CompletedTask;
    }

    async Task OnStarted(IContext context)
    {
        try
        {
            var (tenantId, eventSourceId) = GetIdentifiers(context);

            _eventSourceId = eventSourceId;
            var serviceProvider = await _getServiceProvider(tenantId);
            _aggregateWrapper = ActivatorUtilities.CreateInstance<AggregateWrapper<TAggregate>>(serviceProvider, _eventSourceId);
            if (_idleUnloadTimeout > TimeSpan.Zero)
            {
                context.SetReceiveTimeout(_idleUnloadTimeout);
            }
        }
        catch (Exception e)
        {
            _logger.FailedToCreate(e, typeof(TAggregate));
            Activity.Current?.AddException(e);
            throw;
        }
    }

    static (TenantId, EventSourceId) GetIdentifiers(IContext context)
    {
        return ClusterIdentityMapper.GetTenantAndEventSourceId(context.ClusterIdentity()!);
    }

    async Task OnPerform(Perform<TAggregate> perform, IContext context)
    {
        try
        {
            await _aggregateWrapper!.Perform(perform.Callback, perform.CancellationToken);
            context.Respond(new Try<bool>(true));
        }
        catch (Exception e)
        {
            Activity.Current?.AddException(e);
            context.Respond(new Try<bool>(e));
        }
        finally
        {
            if (_idleUnloadTimeout == TimeSpan.Zero) // 0 means instantly unload
            {
                // ReSharper disable once MethodHasAsyncOverload - awaiting this will deadlock
                context.Poison(context.Self);
            }
        }
    }
    
    async Task OnPerformAndRespond(PerformAndRespond<TAggregate> performAndRespond, IContext context)
    {
        try
        {
            var response = await _aggregateWrapper!.Perform(performAndRespond.Callback, performAndRespond.CancellationToken);
            context.Respond(new Try<object?>(response));
        }
        catch (Exception e)
        {
            Activity.Current?.AddException(e);
            context.Respond(new Try<object?>(e));
        }
        finally
        {
            if (_idleUnloadTimeout == TimeSpan.Zero) // 0 means instantly unload
            {
                // ReSharper disable once MethodHasAsyncOverload - awaiting this will deadlock
                context.Poison(context.Self);
            }
        }
    }
}
