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

[EventType("f8e38583-d9a4-4783-8ac5-29cc0d006e8c")]
public class NestedRedactedEvent
{
    public string SomeId { get; set; }
    
    [RedactablePersonalData]
    public string? SomePii { get; set; }
    
    public Nesting NestedObject { get; set; }
    
    public class Nesting
    {
        [RedactablePersonalData<int>(-1)] public int SomeVal { get; set; }

        [RedactablePersonalData<string>("<fjernet pga gdpr-forespørsel>")]
        public string? SomeImportantPii { get; set; }

        public string SomethingElse { get; set; }

        [RedactablePersonalData] public DateTimeOffset? BirthDate { get; set; }
    }
}

[EventType("f8e38583-d9a4-4783-8ac5-29cc0d006e8c")]
public class RedactedAtTopLevelEvent
{
    public string SomeId { get; set; }
    
    [RedactablePersonalData]
    public string? SomePii { get; set; }
    
    [RedactablePersonalData]
    public Nesting? NestedObject { get; set; }
    
    public class Nesting
    {
        [RedactablePersonalData<int>(-1)] public int SomeVal { get; set; }

        [RedactablePersonalData<string>("<fjernet pga gdpr-forespørsel>")]
        public string? SomeImportantPii { get; set; }

        public string SomethingElse { get; set; }

        [RedactablePersonalData] public DateTimeOffset? BirthDate { get; set; }
    }
}

[EventType("f8e38583-d9a4-4783-8ac5-29cc0d006e8c")]
public class VeryNestedRedactedEvent
{
    public string SomeId { get; set; }
    
    [RedactablePersonalData]
    public string? SomePii { get; set; }
    
    public Nesting NestedObject { get; set; }
    
    public class Nesting
    {
        [RedactablePersonalData<string>("")]
        public string SomeOtherPii { get; set; }
        
        public MoreNesting AnotherOne { get; set; }
    }

    public class MoreNesting
    {
        [RedactablePersonalData<int>(-1)] public int SomeVal { get; set; }

        [RedactablePersonalData<string>("<fjernet pga gdpr-forespørsel>")]
        public string? SomeImportantPii { get; set; }

        public string SomethingElse { get; set; }

        [RedactablePersonalData] public DateTimeOffset? BirthDate { get; set; }
    }
}

[EventType("5577fe91-5955-4b93-98b0-6399647ffdf3")]
public record RedactedRecord(
    [property: RedactablePersonalData] string? RedactedParam,
    [property: RedactablePersonalData<int>(-999)] int AnotherRedactedParam,
    string NonRedactedParam);


[EventType("de1e7e17-bad5-da7a-aaaa-fbc6ec3c0ea6")]
public class SomeRecordRedacted : PersonalDataRedactedForEvent<RedactedRecord>;

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
    public void WhenTypeHasNestedRedactableProperties()
    {
        RedactedType<NestedRedactedEvent>.RedactedProperties
            .Should().BeEquivalentTo(new Dictionary<string, object?>
            {
                { "SomePii", null },
                { "NestedObject.SomeVal", -1 },
                { "NestedObject.SomeImportantPii", "<fjernet pga gdpr-forespørsel>" },
                { "NestedObject.BirthDate", null },
            });
    }
    
    [Fact]
    public void WhenTypeHasBeenRedactedAtTopLevel()
    {
        RedactedType<RedactedAtTopLevelEvent>.RedactedProperties
            .Should().BeEquivalentTo(new Dictionary<string, object?>
            {
                { "SomePii", null },
                { "NestedObject", null },
            });
    }
    
    [Fact]
    public void WhenTypeHasDeeplyNestedRedactableProperties()
    {
        RedactedType<VeryNestedRedactedEvent>.RedactedProperties
            .Should().BeEquivalentTo(new Dictionary<string, object?>
            {
                { "SomePii", null },
                { "NestedObject.SomeOtherPii", "" },
                { "NestedObject.AnotherOne.SomeVal", -1 },
                { "NestedObject.AnotherOne.SomeImportantPii", "<fjernet pga gdpr-forespørsel>" },
                { "NestedObject.AnotherOne.BirthDate", null },
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
            Redactions.TryCreate<RedactedEvent>(reason, redactedBy, out var redactionEvent,
                out var error);

        success.Should().BeTrue();
        error.Should().BeNull();
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
    
    [Fact]
    public void WhenCreatingCustomRedactionEvent()
    {
        var reason = "Some reason";
        var redactedBy = "Some person";
        var success =
            Redactions.TryCreate<RedactedRecord, SomeRecordRedacted>(reason, redactedBy, out var redactionEvent,
                out var error);

        success.Should().BeTrue();
        error.Should().BeNull();
        redactionEvent.Should().NotBeNull();
        redactionEvent!.EventId.Should().Be("5577fe91-5955-4b93-98b0-6399647ffdf3");
        redactionEvent.EventAlias.Should().Be(nameof(RedactedRecord));
        redactionEvent.RedactedProperties.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            { "RedactedParam", null },
            { "AnotherRedactedParam", -999 },
        });
        redactionEvent.RedactedBy.Should().Be(redactedBy);
        redactionEvent.Reason.Should().Be(reason);
    }
    
    

    [Theory]
    [InlineData("", "Some person", "Reason cannot be empty")]
    [InlineData("Some reason", "", "RedactedBy cannot be empty")]
    public void WhenProvidingInsufficientInput(string reason, string redactedBy, string expectedError)
    {
        var success = Redactions.TryCreate<RedactedEvent>(reason, redactedBy, out var redactionEvent,
                out var error);

        success.Should().BeFalse();
        redactionEvent.Should().BeNull();
        error.Should().Be(expectedError);
    }
}
