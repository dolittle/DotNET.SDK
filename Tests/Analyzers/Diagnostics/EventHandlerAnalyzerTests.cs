// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.Diagnostics;

public class EventHandlerAnalyzerTests : AnalyzerTest<EventHandlerAnalyzer>
{
    [Fact]
    public async Task ShouldDetectNothing()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp:""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    public void Handle(SomeEvent evt, EventContext context)
    {
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task ShouldDetectNothingAsync()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using System.Threading.Tasks;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp:""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    public async Task Handle(SomeEvent evt, EventContext context)
    {
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
    }

    [Fact]
    public async Task ShouldDetectInvalidStartFromTimestamp()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, startFromTimestamp:""2023-09-15T07:30Z!"")]
class SomeEventHandler
{
    
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.InvalidTimestamp)
                .WithSpan(5, 86, 5, 125)
                .WithArguments("startFromTimestamp")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldDetectInvalidStopAtTimestamp()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp:""2023-09-15T07:30Z!"")]
class SomeEventHandler
{
    
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.InvalidTimestamp)
                .WithSpan(5, 86, 5, 122)
                .WithArguments("stopAtTimestamp")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldNotDetectDetectTimestampIssues()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFromTimestamp:""2023-09-15T06:25Z"", stopAtTimestamp:""2023-09-15T06:30Z"")]
class SomeEventHandler
{
    
}";

        await VerifyAnalyzerFindsNothingAsync(test);

    }
    
    [Fact]
    public async Task ShouldDetectDetectTimestampIssues()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", stopAtTimestamp:""2023-09-15T06:25Z"", startFromTimestamp:""2023-09-15T06:30Z"")]
class SomeEventHandler
{
    
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.InvalidStartStopTimestamp)
                .WithSpan(5, 2, 5, 131)
                .WithArguments("startFromTimestamp","stopAtTimestamp")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldDetectDetectMissingEventTypeAnnotation()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"")]
class SomeEventHandler
{
    public void Handle(SomeEvent evt, EventContext context)
    {
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Events.MissingAttribute)
                .WithSpan(10, 34, 10, 37)
                .WithArguments("SomeEvent")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldDetectInvalidAccessibilityWhenNotSpecific()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp:""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    void Handle(SomeEvent evt, EventContext context)
    {
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.InvalidAccessibility)
                .WithSpan(11, 10, 11, 16)
                .WithArguments("Handle", "Public")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldDetectInvalidAccessibilityWhenPrivate()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp:""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    private void Handle(SomeEvent evt, EventContext context)
    {
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.InvalidAccessibility)
                .WithSpan(11, 18, 11, 24)
                .WithArguments("Handle", "Public")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldDetectInvalidAccessibilityWhenInternal()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp:""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    internal void Handle(SomeEvent evt, EventContext context)
    {
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.InvalidAccessibility)
                .WithSpan(11, 19, 11, 25)
                .WithArguments("Handle", "Public")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
    [Fact]
    public async Task ShouldDetectMissingEventContext()
    {
        var test = @"
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp:""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    public void Handle(SomeEvent evt)
    {
    }
}";
        DiagnosticResult[] expected =
        [
            Diagnostic(DescriptorRules.Events.MissingEventContext)
                .WithSpan(11, 17, 11, 23)
                .WithArguments("Handle")
        ];

        await VerifyAnalyzerAsync(test, expected);
    }
    
}
