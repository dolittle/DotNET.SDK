// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Loggers;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Benchmarks.Event.Store;

/// <summary>
/// Benchmarks for committing events to the Event Store.
/// </summary>
public class CommitEvents : JobBase
{
    IEventStore _eventStore;
    EventToCommit _eventToCommit;
    EventSourceId _eventSourceId;
    EventType _eventType;
    
    /// <inheritdoc />
    protected override void Setup(IDolittleClient client)
    {
        _eventStore = client.EventStore.ForTenant(ConfiguredTenants.First());
        _eventToCommit = new EventToCommit();
        _eventSourceId = "94fb68dc-9305-4c85-9f27-3a85b686ada4";
        _eventType = new EventType("b2b167be-cbd4-43f5-a8bd-bf45bd226d91");

        if (PreExistingEvents < 1)
        {
            return;
        }

        for (var n = 0; n < PreExistingEvents; n++)
        {
            _eventStore.Commit(_ => _.CreateEvent(_eventToCommit).FromEventSource(_eventSourceId).WithEventType(_eventType)).GetAwaiter().GetResult();
        }
    }
    
    /// <summary>
    /// Gets the number of events that have been committed to the Event Store before the benchmarking commits.
    /// </summary>
    [Params(0, 100, 1000)]
    public int PreExistingEvents { get; set; }
    
    /// <summary>
    /// Gets the number of events to be committed in the benchmarks.
    /// </summary>
    [Params(1, 10, 50, 100)]
    public int EventsToCommit { get; set; }

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public async Task CommitEventsInLoop()
    {
        for (var n = 0; n < EventsToCommit; n++)
        {
            await _eventStore.Commit(_ => _.CreateEvent(_eventToCommit).FromEventSource(_eventSourceId).WithEventType(_eventType));
        }
    }

    /// <summary>
    /// Commits the events in a single batch.
    /// </summary>
    [Benchmark]
    public Task CommitEventsInBatch()
        => _eventStore.Commit(_ =>
        {
            for (var n = 0; n < EventsToCommit; n++)
            {
                _.CreateEvent(_eventToCommit).FromEventSource(_eventSourceId).WithEventType(_eventType);
            }
        });

    /// <summary>
    /// The method that cleans up the environment after each benchmark run.
    /// </summary>
    protected override void Cleanup()
    {
    }

    class EventToCommit
    {
        public string Hello => "World";
    }
}
