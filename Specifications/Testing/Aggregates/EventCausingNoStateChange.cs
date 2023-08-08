using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

[EventType("7d623870-54e5-4a73-a02d-1bac4f906e5a")]
public record EventCausingNoStateChange(string EventSourceId);