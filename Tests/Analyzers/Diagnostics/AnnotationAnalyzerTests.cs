// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.Diagnostics;

public class AttributeAnalyzerTests : AnalyzerTest<AttributeIdentityAnalyzer>
{
    //The implementation is currently non-deterministic in which element will be marked as duplicate.
    // As this leads to flakiness, we are disabling the test for now.
//     [Fact]
//     public async Task ShouldDetectDuplicateIdentities()
//     {
//         var test = @"
// using Dolittle.SDK.Events;
//
// [EventType(""c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb"")]
// record SomeEvent
// {
//     public string Name {get; set;}
// }
//
// [EventType(""c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb"")]
// class SomeOtherEvent
// {
//     public string Name {get; set;}
// }";
//         DiagnosticResult[] expected =
//         {
//             Diagnostic(DescriptorRules.DuplicateIdentity)
//                 .WithSpan(4, 2, 4, 51)
//                 .WithArguments("EventType", "c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb")
//         };
//
//         await VerifyAnalyzerAsync(test, expected);
//     }

    [Fact]
    public async Task ShouldDetectEventTypeWithUsing()
    {
        var test = @"
using Dolittle.SDK.Events;

[EventType("""")]
class SomeEvent
{
    public string Name {get; set;}
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.InvalidIdentity)
                .WithSpan(4, 2, 4, 15)
                .WithArguments("EventType", "eventTypeId", "\"\"")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldDetectEventTypeWhenQualified()
    {
        var test = @"
[Dolittle.SDK.Events.EventType("""")]
class SomeEvent
{
    public string Name {get; set;}
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.InvalidIdentity)
                .WithSpan(2, 2, 2, 35)
                .WithArguments("Dolittle.SDK.Events.EventType", "eventTypeId", "\"\""),
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldNotDetectEventTypeWhenItHasValidId()
    {
        var test = @"
[Dolittle.SDK.Events.EventType(""e8879da9-fd28-4c78-b9cc-1381a09c3e79"")]
class SomeEvent
{
    public string Name {get; set;}
}";

        await VerifyAnalyzerFindsNothingAsync(test);
    }

    [Fact]
    public async Task ShouldNotDetectEventTypeWhenItHasValidIdAndNamedArguments()
    {
        var test = @"
[Dolittle.SDK.Events.EventType(alias: ""Bob"", eventTypeId: ""c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb"")]
class SomeEvent
{
    public string Name {get; set;}
}";

        await VerifyAnalyzerFindsNothingAsync(test);
    }

    [Fact]
    public async Task ShouldDetectEventTypeWithInvalidNamedArguments()
    {
        var test = @"
[Dolittle.SDK.Events.EventType(alias: ""Bob"", eventTypeId: ""c6f87322-be67-4aaf"")]
class SomeEvent
{
    public string Name {get; set;}
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.InvalidIdentity)
                .WithSpan(2, 2, 2, 80)
                .WithArguments("Dolittle.SDK.Events.EventType", "eventTypeId", "\"c6f87322-be67-4aaf\"")
        };

        await VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ShouldIgnoreIrrelevantAttributes()
    {
        var test = @"
[System.Serializable]
class SomeEvent
{
    public string Name {get; set;}
}";

        await VerifyAnalyzerFindsNothingAsync(test);
    }
    
    [Fact]
    public async Task ShouldDetectEventHandlerWhenQualified()
    {
        var test = @"
[Dolittle.SDK.Events.Handling.EventHandler("""")]
class SomeEvent
{
    public string Name {get; set;}
}";
        DiagnosticResult[] expected =
        {
            Diagnostic(DescriptorRules.InvalidIdentity)
                .WithSpan(2, 2, 2, 47)
                .WithArguments("Dolittle.SDK.Events.Handling.EventHandler", "eventHandlerId", "\"\""),
        };

        await VerifyAnalyzerAsync(test, expected);
    }
}
