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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Aggregates.Actors;

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
    readonly IServiceProvider _serviceProvider;
    readonly ILogger<AggregateActor<TAggregate>> _logger;
    AggregateWrapper<TAggregate>? _aggregateWrapper;

    EventSourceId? _eventSourceId;

    public AggregateActor(IServiceProvider serviceProvider, ILogger<AggregateActor<TAggregate>> logger)
    {
        _serviceProvider = serviceProvider;
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
        context.Poison(context.Self);
        return Task.CompletedTask;
    }

    Task OnStarted(IContext context)
    {
        try
        {
            var identity = context.ClusterIdentity();
            _eventSourceId = identity!.Identity;
            _aggregateWrapper = ActivatorUtilities.CreateInstance<AggregateWrapper<TAggregate>>(_serviceProvider, _eventSourceId);
            context.SetReceiveTimeout(TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
