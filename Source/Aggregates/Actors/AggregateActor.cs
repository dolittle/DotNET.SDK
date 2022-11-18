// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Aggregates.Actors;

delegate Task<IServiceProvider> GetServiceProviderForTenant(TenantId tenantId);

class Perform<TAggregate> where TAggregate : AggregateRoot
{
    public Perform(Func<TAggregate, Task> callback, CancellationToken cancellationToken)
    {
        Callback = callback;
        CancellationToken = cancellationToken;
    }

    public Func<TAggregate, Task> Callback { get; }
    public CancellationToken CancellationToken { get; }
}

public class AggregateActor<TAggregate> : IActor where TAggregate : AggregateRoot
{
    readonly GetServiceProviderForTenant _getServiceProvider;
    readonly ILogger<AggregateActor<TAggregate>> _logger;
    AggregateWrapper<TAggregate>? _aggregateWrapper;

    EventSourceId? _eventSourceId;

    // ReSharper disable once StaticMemberInGenericType
    static readonly TimeSpan _idleUnloadTimeout = TimeSpan.FromSeconds(20);

    internal AggregateActor(GetServiceProviderForTenant getServiceProvider, ILogger<AggregateActor<TAggregate>> logger)
    {
        _getServiceProvider = getServiceProvider;
        _logger = logger;
    }

    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            Started => OnStarted(context),
            ReceiveTimeout => OnReceiveTimeout(context),
            Perform<TAggregate> msg => OnPerform(msg, context),
            _ => Task.CompletedTask
        };
    }

    Task OnReceiveTimeout(IContext context)
    {
        _logger.UnloadingAggregate(typeof(TAggregate));
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
            context.SetReceiveTimeout(_idleUnloadTimeout);
        }
        catch (Exception e)
        {
            _logger.FailedToCreate(e, typeof(TAggregate));
            Activity.Current?.RecordError(e);
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
            Activity.Current?.RecordError(e);
            context.Respond(new Try<bool>(e));
        }
    }
}
