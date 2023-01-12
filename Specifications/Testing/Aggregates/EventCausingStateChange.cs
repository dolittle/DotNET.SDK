using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

[EventType("502a8e67-bc6b-4e2f-a6e3-c60822ac950a")]
public record EventCausingStateChange(int NewState);