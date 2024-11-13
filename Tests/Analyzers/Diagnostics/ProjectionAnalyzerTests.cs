// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.Diagnostics;

public class ProjectionAnalyzerTests : AnalyzerTest<ProjectionsAnalyzer>
{
    [Fact]
    public async Task ShouldFindNoIssues()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt)
    {
        Name = evt.Name;
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task ShouldFindPrivateMethod()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    private void On(NameUpdated evt)
    {
        Name = evt.Name;
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Projection.InvalidOnMethodVisibility)
                .WithSpan(14, 5, 17, 6)
                .WithArguments("SomeProjection.On(NameUpdated)")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindNoIssuesWithProjectionContext()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt, ProjectionContext ctx)
    {
        Name = evt.Name;
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task ShouldFindNoIssuesWithEventContext()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt, EventContext ctx)
    {
        Name = evt.Name;
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
    }

//     [Fact]
//     public async Task ShouldFindNonPrivateOnMethod()
//     {
//         var test = @"
// using Dolittle.SDK.Projections;
// using Dolittle.SDK.Events;
//
//
// [EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
// record NameUpdated(string Name);
//
// [Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
// class SomeProjection: ReadModel
// {
//     public string Name {get; set;}
//
//     public void UpdateName(string name)
//     {
//         Apply(new NameUpdated(name));
//     }
//
//     public void On(NameUpdated evt)
//     {
//         Name = evt.Name;
//     }
// }";
//
//         DiagnosticResult[] expected =
//         {
//             Diagnostic(DescriptorRules.Projection.MutationShouldBePrivate)
//                 .WithSpan(19, 5, 22, 6)
//                 .WithArguments("SomeProjection.On(NameUpdated)")
//         };
//
//         await VerifyAnalyzerAsync(test, expected);
//     }

    [Fact]
    public async Task ShouldFindOnMethodWithIncorrectParameters()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}


    public void On(NameUpdated evt, string shouldNotBeHere)
    {
        Name = evt.Name;
    }
}";

        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Projection.InvalidOnMethodParameters)
                .WithSpan(15, 37, 15, 59)
                .WithArguments("SomeProjection.On(NameUpdated, string)")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindDuplicatedOnHandlers()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt)
    {
        Name = evt.Name;
    }

    public void On(NameUpdated evt, ProjectionContext ctx)
    {
        Name = evt.Name;
    }
}";

        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Projection.EventTypeAlreadyHandled)
                .WithSpan(19, 5, 22, 6)
                .WithArguments("NameUpdated")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindOnMethodWithNoParameters()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On()
    {
    }
}";

        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Projection.InvalidOnMethodParameters)
                .WithSpan(14, 5, 16, 6)
                .WithArguments("SomeProjection.On()")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingProjectionAttribute()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt)
    {
        Name = evt.Name;
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Projection.MissingAttribute)
                .WithSpan(9, 7, 9, 21)
                .WithArguments("SomeProjection")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindMissingEventAttribute()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

namespace Test;

record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt)
    {
        Name = evt.Name;
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Events.MissingAttribute)
                .WithSpan(14, 20, 14, 35)
                .WithArguments("Test.NameUpdated")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindDateTimeNow()
    {
        var test = @"
using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}
    public DateTime LastUpdated {get; set;}

    public void On(NameUpdated evt)
    {
        Name = evt.Name;
        LastUpdated = DateTime.Now;
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Projection.MutationUsedCurrentTime)
                .WithSpan(19, 23, 19, 35)
                .WithArguments("DateTime.Now")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindDateTimeOffsetNow()
    {
        var test = @"
using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}
    public DateTimeOffset LastUpdated {get; set;}

    public void On(NameUpdated evt)
    {
        Name = evt.Name;
        LastUpdated = DateTimeOffset.UtcNow;
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Projection.MutationUsedCurrentTime)
                .WithSpan(19, 23, 19, 44)
                .WithArguments("DateTimeOffset.UtcNow")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldFindThrows()
    {
        var test = @"
using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt)
    {
        if(evt.Name == null) throw new ArgumentNullException(nameof(evt.Name));
        Name = evt.Name;
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.ExceptionInMutation)
                .WithSpan(16, 30, 16, 80)
                .WithArguments("throw new ArgumentNullException(nameof(evt.Name)).UtcNow")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldFindThrowIf()
    {
        var test = @"
using System;
using Dolittle.SDK.Projections;
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

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}

    public void On(NameUpdated evt)
    {
        AnArgumentException.ThrowIfNullOrEmpty(evt.Name);
        Name = evt.Name;
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.ExceptionInMutation)
                .WithSpan(25, 9, 25, 57)
                .WithArguments("AnArgumentException.ThrowIfNullOrEmpty(evt.Name)")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }

}
