using Dolittle.SDK.Artifacts;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

// ReSharper disable ClassNeverInstantiated.Global

namespace Dolittle.SDK.Events;

[EventType("0f025f6c-41be-4192-895d-c1c771832bae")]
record SomeEventRecord(string Thing);

[EventType("11e1e3e3-1e1e-4e3e-8e3e-1e1e3e1e3e1e", 2, "Foo")]
record SomeEventClass(string Thing);

public class EventTypeMetadataTests
{
    [Fact]
    public void CanGetEventTypeStatically()
    {
        var eventType = EventTypeMetadata<SomeEventRecord>.EventType;

        using var _ = new AssertionScope();

        eventType.Should().NotBeNull();
        eventType!.Id.Should().Be(new EventTypeId(Guid.Parse("0f025f6c-41be-4192-895d-c1c771832bae")));
        
        eventType.Alias.Should().NotBeNull();
        eventType.Alias!.Value.Should().Be(nameof(SomeEventRecord));
        
        eventType.Generation.Should().Be(Generation.First);
    }
    
    [Fact]
    public void CanGetEventTypeStaticallyWhenAliasAndGenerationSpecified()
    {
        var eventType = EventTypeMetadata<SomeEventClass>.EventType;

        using var _ = new AssertionScope();

        eventType.Should().NotBeNull();
        eventType!.Id.Should().Be(new EventTypeId(Guid.Parse("11e1e3e3-1e1e-4e3e-8e3e-1e1e3e1e3e1e")));
        
        eventType.Alias.Should().NotBeNull();
        eventType.Alias!.Value.Should().Be("Foo");
        
        eventType.Generation.Value.Should().Be(2);
    }
}
