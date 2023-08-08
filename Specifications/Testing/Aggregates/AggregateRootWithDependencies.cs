using System;
using Dolittle.SDK.Aggregates;

namespace Dolittle.SDK.Testing.Aggregates;

[AggregateRoot("14c6d337-96fc-4d39-a19e-641c3d006d10")]
public class AggregateRootWithDependencies : AggregateRoot
{
    public record SomeDependencies(int SomeValue);

    public static Exception TheFailure = new("I failed");

    public AggregateRootWithDependencies(SomeDependencies someDependencies)
    {
        TheDependency = someDependencies;
    }

    public SomeDependencies TheDependency { get; }
    
    public void EventCausingNoStateChange()
    {
        Apply(new EventCausingNoStateChange(EventSourceId.Value));
    }
    public void Fail()
    {
        throw TheFailure;
    }
}