// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.CodeFixes;

public class EventHandlerEventContextCodeFixProviderTests: CodeFixProviderTests<EventHandlerAnalyzer, EventHandlerEventContextCodeFixProvider>
{
    
    [Fact]
    public async Task ShouldGenerateEventContext()
    {
        var test = @"using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp: ""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    public void Handle(SomeEvent evt)
    {
        System.Console.WriteLine(""Hello World"");
    }
}";

        var expected = @"using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp: ""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    public void Handle(SomeEvent evt, EventContext ctx)
    {
        System.Console.WriteLine(""Hello World"");
    }
}";

        var diagnosticResult = Diagnostic(DescriptorRules.Events.MissingEventContext)
            .WithSpan(10, 17, 10, 23)
            .WithArguments("Handle");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task ShouldGenerateEventContextAndNamespace()
    {
        var test = @"using Dolittle.SDK.Events.Handling;

[Dolittle.SDK.Events.EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp: ""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    public void Handle(SomeEvent evt)
    {
        System.Console.WriteLine(""Hello World"");
    }
}";

        var expected = @"using Dolittle.SDK.Events.Handling;
using Dolittle.SDK.Events;

[Dolittle.SDK.Events.EventType(""86dd35ee-cd28-48d9-a0cd-cb2aa11851aa"")]
record SomeEvent();

[EventHandler(""86dd35ee-cd28-48d9-a0cd-cb2aa11851af"", startFrom: ProcessFrom.Latest, stopAtTimestamp: ""2023-09-15T07:30Z"")]
class SomeEventHandler
{
    public void Handle(SomeEvent evt, EventContext ctx)
    {
        System.Console.WriteLine(""Hello World"");
    }
}";

        var diagnosticResult = Diagnostic(DescriptorRules.Events.MissingEventContext)
            .WithSpan(9, 17, 9, 23)
            .WithArguments("Handle");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
}
