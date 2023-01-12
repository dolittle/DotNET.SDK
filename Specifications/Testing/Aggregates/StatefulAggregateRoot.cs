using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

[AggregateRoot("09f7581d-9749-4c6c-87aa-8b8016509f08")]
public class StatefulAggregateRoot : AggregateRoot
{
    public static Exception TheFailure = new("I failed");

    public StatefulAggregateRoot(EventSourceId eventSource)
        : base(eventSource)
    {
    }

    public int TheState { get; private set; }
    
    public void EventCausingNoStateChange()
    {
        Apply(new EventCausingNoStateChange());
    }
    public void EventCausingStateChange(int theNewState)
    {
        Apply(new EventCausingStateChange(theNewState));
    }
    public void Fail()
    {
        throw TheFailure;
    }

    void On(EventCausingStateChange evt)
    {
        TheState = evt.NewState;
    }
}