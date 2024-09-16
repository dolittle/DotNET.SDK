// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;

namespace Dolittle.Benchmarks.SDK.AggregateRoots.with_1_tenant;

public class applying_events_with_1000_events_to_replay : SingleRuntimeSetup
{
    IAggregates _aggregates = null!;
    readonly EventSourceId _aggregateRootEventSource = "aggregate root source"; 
    public override void IterationSetup()
    {
        base.IterationSetup();
        var client = GetConnectedClient(_ => _
            .WithEventTypes(types => types.Register<AnEvent>().Register<LastEvent>())
            .WithAggregateRoots(_ => _
                .Register<AnAggregateRoot>()));
        _aggregates = client.Aggregates.ForTenant(TenantId.Development);
        _aggregates.Get<AnAggregateRoot>(_aggregateRootEventSource).Perform(_ =>
        {
            for (var i = 0; i < 1000; i++)
            {
                _.DoSomething();
            }
        }).Wait();
    }

    [Benchmark]
    public async Task Applying1EventWith1000EventsToReplay()
    {
        await _aggregates.Get<AnAggregateRoot>(_aggregateRootEventSource).Perform(_ => _.Finish()).ConfigureAwait(false);
    }
    
    [Benchmark]
    public async Task Applying100EventsWith1000EventsToReplay()
    {
        await _aggregates.Get<AnAggregateRoot>(_aggregateRootEventSource).Perform(_ =>
        {
            for (var i = 0; i < 99; i++)
            {
                _.DoSomething();
            }
            _.Finish();
        }).ConfigureAwait(false);
    }

    [Benchmark]
    public async Task Applying1Event100TimesWith1000EventsToReplay()
    {
        for (var i = 0; i < 99; i++)
        {
            await _aggregates.Get<AnAggregateRoot>(_aggregateRootEventSource).Perform(_ => _.DoSomething()).ConfigureAwait(false);
        }
        await _aggregates.Get<AnAggregateRoot>(_aggregateRootEventSource).Perform(_ => _.Finish()).ConfigureAwait(false);
    }
}
