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
class SomeProjection: ProjectionBase
{
    public string Name {get; set;}

    private void On(NameUpdated evt)
    {
        Name = evt.Name;
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
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
class SomeProjection: ProjectionBase
{
    public string Name {get; set;}

    private void On(NameUpdated evt, ProjectionContext ctx)
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
class SomeProjection: ProjectionBase
{
    public string Name {get; set;}

    private void On(NameUpdated evt, EventContext ctx)
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
// class SomeProjection: ProjectionBase
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
class SomeProjection: ProjectionBase
{
    public string Name {get; set;}


    private void On(NameUpdated evt, string shouldNotBeHere)
    {
        Name = evt.Name;
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Projection.InvalidOnMethodParameters)
                .WithSpan(15, 38, 15, 60)
                .WithArguments("SomeProjection.On(NameUpdated, string)")
        };

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
class SomeProjection: ProjectionBase
{
    public string Name {get; set;}

    void On(NameUpdated evt)
    {
        Name = evt.Name;
    }

    void On(NameUpdated evt, ProjectionContext ctx)
    {
        Name = evt.Name;
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Projection.EventTypeAlreadyHandled)
                .WithSpan(19, 5, 22, 6)
                .WithArguments("NameUpdated")
        };

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
class SomeProjection: ProjectionBase
{
    public string Name {get; set;}

    void On()
    {
    }
}";

        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Projection.InvalidOnMethodParameters)
                .WithSpan(14, 5, 16, 6)
                .WithArguments("SomeProjection.On()")
        };

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

class SomeProjection: ProjectionBase
{
    public string Name {get; set;}

    private void On(NameUpdated evt)
    {
        Name = evt.Name;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Projection.MissingAttribute)
                .WithSpan(9, 7, 9, 21)
                .WithArguments("SomeProjection")
        };

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
class SomeProjection: ProjectionBase
{
    public string Name {get; set;}

    private void On(NameUpdated evt)
    {
        Name = evt.Name;
    }
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.Events.MissingAttribute)
                .WithSpan(14, 21, 14, 36)
                .WithArguments("Test.NameUpdated"),
        };

        await VerifyAnalyzerAsync(test, expected);
    }
}
