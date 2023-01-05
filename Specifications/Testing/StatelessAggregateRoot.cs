using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Testing;

[AggregateRoot("cc4a2d9c-e7b4-4e5e-afcd-d02b9b144eea")]
public class StatelessAggregateRoot : AggregateRoot
{
    
}

// [EventType()]
// public record EventCausingNoStateChange;
//
