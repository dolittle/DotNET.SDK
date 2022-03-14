// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.Benchmarks.SDK.AggregateRoots;

[AggregateRoot("df4b9fb2-f408-4952-abc8-653e948cf681")]
public class AnAggregateRoot : AggregateRoot
{
    int num_events_applied;
    bool finished;
    
    public AnAggregateRoot(EventSourceId eventSourceId)
        : base(eventSourceId)
    {
    }

    public void DoSomething()
    {
        Apply(new AnEvent());
    }
    public void Finish()
    {
        Apply(new LastEvent());
    }

    void On(AnEvent evt)
        => num_events_applied++;

    void On(LastEvent evt)
    {
        num_events_applied++;
        finished = true;
    }
}
