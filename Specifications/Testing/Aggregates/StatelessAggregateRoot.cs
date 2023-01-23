using System;
using System.Runtime.CompilerServices;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Dolittle.SDK.Testing.Aggregates;

namespace Dolittle.SDK.Testing.Aggregates;

[AggregateRoot("cc4a2d9c-e7b4-4e5e-afcd-d02b9b144eea")]
public class StatelessAggregateRoot : AggregateRoot
{
    public static Exception TheFailure = new("I failed");
    
    public StatelessAggregateRoot(EventSourceId eventSource)
        : base(eventSource)
    {
    }

    public void EventCausingNoStateChange()
    {
        Apply(new EventCausingNoStateChange());
    }
    public void Fail()
    {
        throw TheFailure;
    }
}