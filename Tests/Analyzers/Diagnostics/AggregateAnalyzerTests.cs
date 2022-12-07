// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.Diagnostics;

public class AggregateAnalyzerTests : AnalyzerTest<AggregateAnalyzer>
{
    [Fact]
    public async Task ShouldFindNoIssues()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
    }

    [Fact]
    public async Task ShouldFindNonPrivateMutation()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    public void On(NameUpdated @event)
    {
        Name = @event.Name;
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationShouldBePrivate)
                .WithSpan(19, 5, 22, 6)
                .WithArguments("SomeAggregate.On(NameUpdated)")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMutationWithIncorrectParameters()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event, string shouldNotBeHere)
    {
        Name = @event.Name;
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationHasIncorrectNumberOfParameters)
                .WithSpan(19, 5, 22, 6)
                .WithArguments("SomeAggregate.On(NameUpdated, string)")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMutationWithNoParameters()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On()
    {
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MissingMutation)
                .WithSpan(16, 9, 16, 37)
                .WithArguments("NameUpdated"),
            Diagnostic(DescriptorRules.Aggregate.MutationHasIncorrectNumberOfParameters)
                .WithSpan(19, 5, 21, 6)
                .WithArguments("SomeAggregate.On()")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingAggregateAttribute()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MissingAttribute)
                .WithSpan(9, 7, 9, 20)
                .WithArguments("SomeAggregate")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingEventAttribute()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Test;

record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Events.MissingAttribute)
                .WithSpan(16, 9, 16, 37)
                .WithArguments("Test.NameUpdated"),
            Diagnostic(DescriptorRules.Events.MissingAttribute)
                .WithSpan(19, 21, 19, 39)
                .WithArguments("Test.NameUpdated")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingMutationFromConstructor()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MissingMutation)
                .WithSpan(16, 9, 16, 37)
                .WithArguments("NameUpdated")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingMutationWhenNested()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        if (name != Name)
        {
            Apply(new NameUpdated(name));
        }
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MissingMutation)
                .WithSpan(18, 13, 18, 41)
                .WithArguments("NameUpdated")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingMutation()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""613cf27a-c58a-4865-86ce-8f40e6a7bae6"")]
public class SomeAggregate : AggregateRoot
{
    private string _name = """";

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new System.ArgumentNullException(nameof(name));
        }

        if (name != _name)
        {
            Apply(new NameUpdated(name));
        }
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MissingMutation)
                .WithSpan(22, 13, 22, 41)
                .WithArguments("NameUpdated")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingMutationFromVariable()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        var @event = new NameUpdated(name);

        Apply(@event);
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MissingMutation)
                .WithSpan(18, 9, 18, 22)
                .WithArguments("NameUpdated")
        };

        await VerifyAnalyzerAsync(test, expected);
    }
}
