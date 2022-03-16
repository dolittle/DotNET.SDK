// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;

namespace Dolittle.Benchmarks.SDK.AggregateRoots.with_1_tenant;

public class applying_events_with_100_events_to_replay : SingleRuntimeSetup
{
    IAggregates _aggregates;
    EventSourceId aggregate_root_event_source = "aggregate root source"; 
    public override void IterationSetup()
    {
        base.IterationSetup();
        var client = GetConnectedClient(_ => _
            .WithEventTypes(_ => _.Register<AnEvent>().Register<LastEvent>())
            .WithAggregateRoots(_ => _
                .Register<AnAggregateRoot>()));
        _aggregates = client.Aggregates.ForTenant(TenantId.Development);
        _aggregates.Get<AnAggregateRoot>(aggregate_root_event_source).Perform(_ =>
        {
            for (var i = 0; i < 100; i++)
            {
                _.DoSomething();
            }
        }).Wait();
    }

    [Benchmark]
    public async Task Applying1EventWith100EventsToReplay()
    {
        await _aggregates.Get<AnAggregateRoot>(aggregate_root_event_source).Perform(_ => _.Finish()).ConfigureAwait(false);
    }
    
    [Benchmark]
    public async Task Applying100EventsWith100EventsToReplay()
    {
        await _aggregates.Get<AnAggregateRoot>(aggregate_root_event_source).Perform(_ =>
        {
            for (var i = 0; i < 99; i++)
            {
                _.DoSomething();
            }
            _.Finish();
        }).ConfigureAwait(false);
    }

    [Benchmark]
    public async Task Applying1Event100TimesWith100EventsToReplay()
    {
        for (var i = 0; i < 99; i++)
        {
            await _aggregates.Get<AnAggregateRoot>(aggregate_root_event_source).Perform(_ => _.DoSomething()).ConfigureAwait(false);
        }
        await _aggregates.Get<AnAggregateRoot>(aggregate_root_event_source).Perform(_ => _.Finish()).ConfigureAwait(false);
    }
}
