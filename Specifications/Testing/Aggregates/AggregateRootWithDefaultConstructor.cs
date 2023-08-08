using System;
using Dolittle.SDK.Aggregates;

namespace Dolittle.SDK.Testing.Aggregates;

[AggregateRoot("7745d021-45d7-46b1-84c4-ea0fc871b84a")]
public class AggregateRootWithDefaultConstructor : AggregateRoot
{
    public static Exception TheFailure = new("I failed");

    public void EventCausingNoStateChange()
    {
        Apply(new EventCausingNoStateChange(EventSourceId.Value));
    }
    
    public void Fail()
    {
        throw TheFailure;
    }
}