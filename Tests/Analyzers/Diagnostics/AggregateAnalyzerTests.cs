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
    public SomeAggregate(){
        Name = ""John Doe"";
    }

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
    public async Task ShouldFindNonPrivateOnMethod()
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
    public async Task ShouldFindOnMethodWithIncorrectParameters()
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
    public async Task ShouldFindOnMethodWithNoParameters()
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
    public async Task ShouldFindMissingOnMethodFromConstructor()
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
    public async Task ShouldFindMissingOnMethodWhenNested()
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
    public async Task ShouldFindMissingOnMethod()
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
    public async Task ShouldFindMissingOnMethodFromVariable()
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

    [Fact]
    public async Task ShouldFindApplyInOnMethod()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3c"")]
record InvalidThingHappening(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On(NameUpdated @event)
    {
        Name = @event.Name;
        Apply(new InvalidThingHappening(""This should not be here""));
    }

    void On(InvalidThingHappening @event)
    {
        Name = @event.Name;
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationsCannotProduceEvents)
                .WithSpan(25, 9, 25, 68)
                .WithArguments("Apply(new InvalidThingHappening(\"This should not be here\"))")
        };

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindApplyInOnMethodWithThis()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3c"")]
record InvalidThingHappening(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On(NameUpdated @event)
    {
        Name = @event.Name;
        this.Apply(new InvalidThingHappening(""This should not be here""));
    }

    void On(InvalidThingHappening @event)
    {
        Name = @event.Name;
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationsCannotProduceEvents)
                .WithSpan(25, 9, 25, 19)
                .WithArguments("Apply(new InvalidThingHappening(\"This should not be here\"))")
        };

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindApplyInOnMethodWithBase()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3c"")]
record InvalidThingHappening(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On(NameUpdated @event)
    {
        Name = @event.Name;
        base.Apply(new InvalidThingHappening(""This should not be here""));
    }

    void On(InvalidThingHappening @event)
    {
        Name = @event.Name;
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationsCannotProduceEvents)
                .WithSpan(25, 9, 25, 19)
                .WithArguments("Apply(new InvalidThingHappening(\"This should not be here\"))")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldNotFindApplyWhenTargetIsNotAggregate()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name){
    public void Apply(){
        System.Console.WriteLine(""Not relevant"");
    }
}


[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On(NameUpdated @event)
    {
        Name = @event.Name;
        @event.Apply();
    }
}";

        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task ShouldFindInvalidMutationOnProperty()
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
        Name = name;
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
    }
}";
        
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.PublicMethodsCannotMutateAggregateState)
                .WithSpan(17, 9, 17, 13)
                .WithArguments("Name = name;")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    
    [Fact]
    public async Task ShouldFindInvalidMutationOnThisProperty()
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
        this.Name = name;
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
    }
}";
        
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.PublicMethodsCannotMutateAggregateState)
                .WithSpan(17, 9, 17, 18)
                .WithArguments("this.Name = name;")
        };

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindInvalidMutationOnField()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    string? _name;

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
        _name = name;
    }

    private void On(NameUpdated @event)
    {
        _name = @event.Name;
    }
}";
        
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.PublicMethodsCannotMutateAggregateState)
                .WithSpan(17, 9, 17, 14)
                .WithArguments("_name = name;")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    
    [Fact]
    public async Task ShouldFindInvalidMutationOnThisField()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    string? _name;

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
        this._name = name;
    }

    private void On(NameUpdated @event)
    {
        this._name = @event.Name;
    }
}";
        
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.PublicMethodsCannotMutateAggregateState)
                .WithSpan(17, 9, 17, 19)
                .WithArguments("this._name = name;")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindDateTimeUsage()
    {
        var test = @"
using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public SomeAggregate(){
        Name = ""John Doe"";
    }

    string Name {get; set;}
    DateTime Timestamp { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
        Timestamp = DateTime.UtcNow;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationsCannotUseCurrentTime)
                .WithSpan(28, 21, 28, 36)
                .WithArguments("DateTime.UtcNow")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindDateTimeUsageWhenQualified()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public SomeAggregate(){
        Name = ""John Doe"";
    }

    string Name {get; set;}
    System.DateTime Timestamp { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
        Timestamp = System.DateTime.UtcNow;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationsCannotUseCurrentTime)
                .WithSpan(27, 21, 27, 43)
                .WithArguments("System.DateTime.UtcNow")
        };

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindDateTimeOffsetUsageWhenQualified()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public SomeAggregate(){
        Name = ""John Doe"";
    }

    string Name {get; set;}
    System.DateTimeOffset Timestamp { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        Name = @event.Name;
        Timestamp = System.DateTimeOffset.UtcNow;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Aggregate.MutationsCannotUseCurrentTime)
                .WithSpan(27, 21, 27, 49)
                .WithArguments("System.DateTimeOffset.UtcNow")
        };

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindThrows()
    {
        var test = @"
using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public SomeAggregate(){
        Name = ""John Doe"";
    }

    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        if(string.IsNullOrWhiteSpace(@event.Name))
        {
            throw new System.ArgumentNullException(nameof(@event.Name));
        }
        Name = @event.Name;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.ExceptionInMutation)
                .WithSpan(28, 13, 28, 73)
                .WithArguments("throw new System.ArgumentNullException(nameof(@event.Name))")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindThrowIf()
    {
        var test = @"
using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

// Stand-in since we are targeting .NET standard
public class AnArgumentException
{
    public static void ThrowIfNullOrEmpty(string value)
    {
        if(string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
    }
}

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public SomeAggregate(){
        Name = ""John Doe"";
    }

    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event)
    {
        AnArgumentException.ThrowIfNullOrEmpty(@event.Name);
        Name = @event.Name;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.ExceptionInMutation)
                .WithSpan(34, 9, 34, 60)
                .WithArguments("AnArgumentException.ThrowIfNullOrEmpty(@event.Name)")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

}
