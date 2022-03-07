// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Tenancy;

namespace Dolittle.Benchmarks.SDK.EventStore.with_1_tenant;

public class committing_100_events_with_warmup : SingleRuntimeSetup
{
    IEventStore _eventStore;

    public override void IterationSetup()
    {
        base.IterationSetup();
        var client = GetConnectedClient();
        _eventStore = client.EventStore.ForTenant(TenantId.Development);
        _eventStore.Commit(_ => _.CreateEvent(new some_event())
            .FromEventSource("source")
            .WithEventType("3065740c-f2e3-4ef4-9738-fe2b1a104399")).Wait();
    }

    [Benchmark]
    public async Task Commit100Loop()
    {
        for (var i = 0; i < 100; i++)
        {
            await _eventStore.Commit(_ => _.CreateEvent(new some_event())
                    .FromEventSource("source")
                    .WithEventType("3065740c-f2e3-4ef4-9738-fe2b1a104399"))
                .ConfigureAwait(false);
        }
    }
    [Benchmark]
    public async Task Commit100()
    {
        var @event = new some_event();
        await _eventStore.Commit(_ =>
        {
            for (var i = 0; i < 100; i++)
            {
                _.CreateEvent(@event)
                    .FromEventSource("source")
                    .WithEventType("3065740c-f2e3-4ef4-9738-fe2b1a104399");
            }
        });
    }
}
