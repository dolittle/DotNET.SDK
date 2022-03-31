// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System.Threading.Tasks;
//using BenchmarkDotNet.Attributes;
//using Dolittle.SDK.Aggregates.Builders;
//using Dolittle.SDK.Tenancy;
//
//namespace Dolittle.Benchmarks.SDK.AggregateRoots.with_1_tenant;
//
//public class applying_events_with_nothing_to_replay : SingleRuntimeSetup
//{
//    IAggregates _aggregates;
//    public override void IterationSetup()
//    {
//        base.IterationSetup();
//        var client = GetConnectedClient(_ => _
//            .WithEventTypes(_ => _.Register<AnEvent>().Register<LastEvent>())
//            .WithAggregateRoots(_ => _
//                .Register<AnAggregateRoot>()));
//        _aggregates = client.Aggregates.ForTenant(TenantId.Development);
//        client.EventStore.ForTenant(TenantId.Development).CommitEvent(new AnEvent(), "source").Wait();
//    }
//
//    [Benchmark]
//    public async Task Applying1EventWithNothingToReplay()
//    {
//        await _aggregates.Get<AnAggregateRoot>("an aggregate").Perform(_ => _.Finish()).ConfigureAwait(false);
//    }
//    
//    [Benchmark]
//    public async Task Applying100EventsWithNothingToReplay()
//    {
//        await _aggregates.Get<AnAggregateRoot>("an aggregate").Perform(_ =>
//        {
//            for (var i = 0; i < 99; i++)
//            {
//                _.DoSomething();
//            }
//            _.Finish();
//        }).ConfigureAwait(false);
//    }
//    
//    [Benchmark]
//    public async Task Applying1Event100TimesWithNothingToReplay()
//    {
//        for (var i = 0; i < 99; i++)
//        {
//            await _aggregates.Get<AnAggregateRoot>("an aggregate").Perform(_ => _.DoSomething()).ConfigureAwait(false);
//        }
//        await _aggregates.Get<AnAggregateRoot>("an aggregate").Perform(_ => _.Finish()).ConfigureAwait(false);
//    }
//}
