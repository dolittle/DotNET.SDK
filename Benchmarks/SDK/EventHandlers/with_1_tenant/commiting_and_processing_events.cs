// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Tenancy;

namespace Dolittle.Benchmarks.SDK.EventHandlers.with_1_tenant;

public class commiting_and_processing_events : SingleRuntimeSetup
{
    IEventStore _eventStore;
    TaskCompletionSource<bool> _finishedProcessing;
    AnEvent anEvent = new();
    LastEvent lastEvent = new();

    public override void IterationSetup()
    {
        base.IterationSetup();
        var client = GetConnectedClient(_ => _
            .WithEventTypes(_ => _.Register<AnEvent>().Register<LastEvent>())
            .WithEventHandlers(_ => _
                .Create("63c974e5-1381-4757-a5de-04ef9d729d16")
                .Partitioned()
                .Handle<AnEvent>((evt, ctx) =>
                {
                })
                .Handle<LastEvent>((evt, ctx) =>
                {
                    _finishedProcessing.SetResult(true);
                })));
        _eventStore = client.EventStore.ForTenant(TenantId.Development);
        _finishedProcessing = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _eventStore.CommitEvent(anEvent, "source").Wait();
    }

    [Benchmark]
    public async Task CommitAndProcess1Event()
    {
        _eventStore.CommitEvent(lastEvent, "source");
        await _finishedProcessing.Task.ConfigureAwait(false);
    }

    [Benchmark]
    public async Task CommitAndProcess100Event()
    {
        _eventStore.Commit(_ =>
        {
            for (var i = 0; i < 99; i++)
            {
                _.CreateEvent(anEvent).FromEventSource("source");
            }
            _.CreateEvent(lastEvent).FromEventSource("source");
        });
        await _finishedProcessing.Task.ConfigureAwait(false);
    }
    [Benchmark]
    public async Task CommitAndProcess1000Event()
    {
        _eventStore.Commit(_ =>
        {
            for (var i = 0; i < 999; i++)
            {
                _.CreateEvent(anEvent).FromEventSource("source");
            }
            _.CreateEvent(lastEvent).FromEventSource("source");
        });
        await _finishedProcessing.Task.ConfigureAwait(false);
    }
}
