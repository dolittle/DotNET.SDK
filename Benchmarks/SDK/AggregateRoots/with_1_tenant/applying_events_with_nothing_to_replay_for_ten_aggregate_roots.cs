// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;

namespace Dolittle.Benchmarks.SDK.AggregateRoots.with_1_tenant;

public class applying_events_with_nothing_to_replay_for_ten_aggregate_roots : SingleRuntimeSetup
{
    IAggregates _aggregates = null!;
    public override void IterationSetup()
    {
        base.IterationSetup();
        var client = GetConnectedClient(builder => builder
            .WithEventTypes(types => types.Register<AnEvent>().Register<LastEvent>())
            .WithAggregateRoots(roots => roots.Register<AnAggregateRoot>()));
        _aggregates = client.Aggregates.ForTenant(TenantId.Development);
        client.EventStore.ForTenant(TenantId.Development).CommitEvent(new AnEvent(), "source").Wait();
    }

    [Benchmark]
    public Task Applying1EventWithNothingToReplay()
    {
        return Task.WhenAll(
            _aggregates.Get<AnAggregateRoot>("aggregate1").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate2").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate3").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate4").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate5").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate6").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate7").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate8").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate9").Perform(_ => _.Finish()),
            _aggregates.Get<AnAggregateRoot>("aggregate10").Perform(_ => _.Finish()));
    }
    
    [Benchmark]
    public Task Applying100EventsWithNothingToReplay()
    {
        return Task.WhenAll(
            Apply100Events("aggregate1"),
            Apply100Events("aggregate2"),
            Apply100Events("aggregate3"),
            Apply100Events("aggregate4"),
            Apply100Events("aggregate5"),
            Apply100Events("aggregate6"),
            Apply100Events("aggregate7"),
            Apply100Events("aggregate8"),
            Apply100Events("aggregate9"),
            Apply100Events("aggregate10"));
    }

    Task Apply100Events(EventSourceId eventSource)
    {
        return _aggregates.Get<AnAggregateRoot>(eventSource).Perform(_ =>
        {
            for (var i = 0; i < 99; i++)
            {
                _.DoSomething();
            }
            _.Finish();
        });
    }

    [Benchmark]
    public Task Applying1Event100TimesWithNothingToReplay()
    {
        return Task.WhenAll(
            Apply100Stupid("aggregate1"),
            Apply100Stupid("aggregate2"),
            Apply100Stupid("aggregate3"),
            Apply100Stupid("aggregate4"),
            Apply100Stupid("aggregate5"),
            Apply100Stupid("aggregate6"),
            Apply100Stupid("aggregate7"),
            Apply100Stupid("aggregate8"),
            Apply100Stupid("aggregate9"),
            Apply100Stupid("aggregate10"));
    }

    async Task Apply100Stupid(EventSourceId eventSource)
    {
        for (var i = 0; i < 99; i++)
        {
            await _aggregates.Get<AnAggregateRoot>(eventSource).Perform(_ => _.DoSomething()).ConfigureAwait(false);
        }
        await _aggregates.Get<AnAggregateRoot>(eventSource).Perform(_ => _.Finish()).ConfigureAwait(false);
    }
}
