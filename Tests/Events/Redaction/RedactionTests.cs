using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;
using FluentAssertions;
using Xunit;

namespace Events.Tests.Redaction;

[EventType("fa553ad4-9feb-4ae8-915b-5a7cb27b549e")]
public class NonRedactedEvent
{
    public string SomeName { get; set; }
}

[EventType("fa553ad4-9feb-4ae8-915b-5a7cb27b549e")]
public record NonRedactedRecord(string SomeName);

[EventType("f8e38583-d9a4-4783-8ac5-29cc0d006e8b")]
public class RedactedEvent
{
    [RedactablePersonalData<int>(-1)] public int SomeVal { get; set; }

    [RedactablePersonalData<string>("<fjernet pga gdpr-forespørsel>")]
    public string? SomeImportantPii { get; set; }

    public string SomethingElse { get; set; }

    [RedactablePersonalData] public DateTimeOffset? BirthDate { get; set; }
}

[EventType("5577fe91-5955-4b93-98b0-6399647ffdf3")]
public record RedactedRecord(
    [property: RedactablePersonalData] string? RedactedParam,
    [property: RedactablePersonalData<int>(-999)] int AnotherRedactedParam,
    string NonRedactedParam);

public class RedactionTests
{
    [Fact]
    public void WhenTypeHasNoRedactedProperties()
    {
        RedactedType<NonRedactedEvent>.RedactedProperties
            .Should().BeEmpty();
    }

    [Fact]
    public void WhenTypeHasRedactableProperties()
    {
        RedactedType<RedactedEvent>.RedactedProperties
            .Should().BeEquivalentTo(new Dictionary<string, object?>
            {
                { "SomeVal", -1 },
                { "SomeImportantPii", "<fjernet pga gdpr-forespørsel>" },
                { "BirthDate", null },
            });
    }

    [Fact]
    public void WhenRecordHasRedactableProperties()
    {
        RedactedType<RedactedRecord>.RedactedProperties
            .Should().BeEquivalentTo(new Dictionary<string, object?>
            {
                { "RedactedParam", null },
                { "AnotherRedactedParam", -999 },
            });
    }

    [Fact]
    public void WhenRecordHasNoRedactableProperties()
    {
        RedactedType<NonRedactedRecord>.RedactedProperties
            .Should().BeEmpty();
    }

    [Fact]
    public void WhenCreatingRedactionEvent()
    {
        var reason = "Some reason";
        var redactedBy = "Some person";
        var success =
            PersonalDataRedactedForEvent.TryCreate<RedactedEvent>(reason, redactedBy, out var redactionEvent,
                out var error);

        success.Should().BeTrue();
        redactionEvent.Should().NotBeNull();
        redactionEvent!.EventId.Should().Be("f8e38583-d9a4-4783-8ac5-29cc0d006e8b");
        redactionEvent.EventAlias.Should().Be(nameof(RedactedEvent));
        redactionEvent.RedactedProperties.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            { "SomeVal", -1 },
            { "SomeImportantPii", "<fjernet pga gdpr-forespørsel>" },
            { "BirthDate", null },
        });
        redactionEvent.RedactedBy.Should().Be(redactedBy);
        redactionEvent.Reason.Should().Be(reason);
    }

    [Theory]
    [InlineData("", "Some person", "Reason cannot be empty")]
    [InlineData("Some reason", "", "RedactedBy cannot be empty")]
    public void WhenProvidingInsufficientInput(string reason, string redactedBy, string expectedError)
    {
        var success =
            PersonalDataRedactedForEvent.TryCreate<RedactedEvent>(reason, redactedBy, out var redactionEvent,
                out var error);

        success.Should().BeFalse();
        redactionEvent.Should().BeNull();
        error.Should().Be(expectedError);
    }
}
