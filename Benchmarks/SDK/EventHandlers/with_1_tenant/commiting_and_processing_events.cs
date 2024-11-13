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
    IEventStore? _eventStore;
    TaskCompletionSource<bool>? _finishedProcessing;
    readonly AnEvent _anEvent = new();
    readonly LastEvent _lastEvent = new();

    int _i = 0;
    EventSourceId[]? _eventSources;
    
    public override void IterationSetup()
    {
        base.IterationSetup();
        _finishedProcessing = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var client = GetConnectedClient(builder => builder
            .WithEventTypes(types => types.Register<AnEvent>().Register<LastEvent>())
            .WithEventHandlers(handlers => handlers
                .Create("63c974e5-1381-4757-a5de-04ef9d729d16")
                .WithConcurrency(Concurrency)
                .Partitioned()
                .Handle<AnEvent>((_, _) => { })
                .Handle<LastEvent>((_, _) => { _finishedProcessing.SetResult(true); })));
        _eventStore = client.EventStore.ForTenant(TenantId.Development);
        _eventStore.CommitEvent(_anEvent, "source").Wait();
        _eventSources = new EventSourceId[EventSourceIds];
        for (var i = 0; i < EventSourceIds; i++)
        {
            _eventSources[i] = $"source_{i}";
        }
    }

    EventSourceId NextEventSource()
    {
        var next = _i % EventSourceIds;
        _i++;
        return _eventSources![next];
    }

    [Params(1, 100)] public int Concurrency { get; set; }

    public int EventSourceIds => Concurrency; // this allows the concurrent processing to be maximally performant, since each event source are always processed sequentially

    [Params(1, 100, 1000)] public int Events { get; set; }
    
    [Benchmark]
    public async Task CommitAndProcessEvents()
    {
        _ = _eventStore!.Commit(_ =>
        {
            for (var i = 1; i < Events; i++)
            {
                _.CreateEvent(_anEvent).FromEventSource(NextEventSource());
            }

            _.CreateEvent(_lastEvent).FromEventSource(NextEventSource());
        });
        await _finishedProcessing!.Task.ConfigureAwait(false);
    }
}
