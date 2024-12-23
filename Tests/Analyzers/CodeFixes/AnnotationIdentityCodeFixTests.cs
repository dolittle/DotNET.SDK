﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.CodeFixes;

public class AttributeIdentityCodeFixTests : CodeFixProviderTests<AttributeIdentityAnalyzer, AttributeIdentityCodeFixProvider>
{
    [Fact]
    public async Task FixAttributeWithInvalidIdentity()
    {
        var test = @"
using Dolittle.SDK.Events;

[EventType("""")]
class SomeEvent
{
    public string Name {get; set;}
}";

        var expected = @"
using Dolittle.SDK.Events;

[EventType(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeEvent
{
    public string Name {get; set;}
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
        var diagnosticResult = Diagnostic(DescriptorRules.InvalidIdentity)
            .WithSpan(4, 2, 4, 15)
            .WithArguments("EventType", "eventTypeId", "");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }

    [Fact]
    public async Task FixEventTypeAttributeWithNoIdentity()
    {
        var test = @"
using Dolittle.SDK.Events;

[EventType]
class SomeEvent
{
    public string Name {get; set;}
}";

        var expected = @"
using Dolittle.SDK.Events;

[EventType(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeEvent
{
    public string Name {get; set;}
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
        var diagnosticResult = DiagnosticResult.CompilerError("CS7036")
            .WithSpan(4, 2, 4, 11)
            .WithArguments("eventTypeId", "Dolittle.SDK.Events.EventTypeAttribute.EventTypeAttribute(string, uint, string?)");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task FixEventHandlerAttributeWithNoIdentity()
    {
        var test = @"
using Dolittle.SDK.Events.Handling;

[EventHandler]
class SomeHandler
{
    public string Name {get; set;}
}";

        var expected = @"
using Dolittle.SDK.Events.Handling;

[EventHandler(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeHandler
{
    public string Name {get; set;}
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
        var diagnosticResult = DiagnosticResult.CompilerError("CS7036")
            .WithSpan(4, 2, 4, 14)
            .WithArguments("eventHandlerId", "Dolittle.SDK.Events.Handling.EventHandlerAttribute.EventHandlerAttribute(string, bool, string?, string?, int, Dolittle.SDK.Events.Handling.ProcessFrom, string?, string?)");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }

    [Fact]
    public async Task FixProjectionAttributeWithNoIdentity()
    {
        var test = @"
using Dolittle.SDK.Projections;

[Projection]
class SomeProjection: ReadModel
{
    public string Name {get; set;}
}";

        var expected = @"
using Dolittle.SDK.Projections;

[Projection(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeProjection: ReadModel
{
    public string Name {get; set;}
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
        var diagnosticResult = DiagnosticResult.CompilerError("CS7036")
            .WithSpan(4, 2, 4, 12)
            .WithArguments("projectionId", "Dolittle.SDK.Projections.ProjectionAttribute.ProjectionAttribute(string, string?, string?, string?, bool)");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task FixAggregateRootAttributeWithNoIdentity()
    {
        var test = @"
using Dolittle.SDK.Aggregates;

[AggregateRoot]
class SomeAggregate: AggregateRoot
{
}";

        var expected = @"
using Dolittle.SDK.Aggregates;

[AggregateRoot(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeAggregate: AggregateRoot
{
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";

        var diagnosticResult = DiagnosticResult.CompilerError("CS7036")
            .WithSpan(4, 2, 4, 15)
            .WithArguments("id", "Dolittle.SDK.Aggregates.AggregateRootAttribute.AggregateRootAttribute(string, string?)");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }    
    [Fact]
    public async Task FixesAttributeWithInvalidIdentityWithNamedArguments()
    {
        var test = @"
using Dolittle.SDK.Events;

[EventType(eventTypeId: """")]
class SomeEvent
{
    public string Name {get; set;}
}";

        var expected = @"
using Dolittle.SDK.Events;

[EventType(eventTypeId: ""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeEvent
{
    public string Name {get; set;}
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";

        var diagnosticResult = Diagnostic(DescriptorRules.InvalidIdentity)
            .WithSpan(4, 2, 4, 28)
            .WithArguments("EventType", "eventTypeId", "");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task FixesAttributeWithInvalidIdentityWithNamedArgumentsInNonDefaultPosition()
    {
        var test = @"
using Dolittle.SDK.Events;

[EventType(alias: ""Foo"", eventTypeId: """")]
class SomeEvent
{
    public string Name {get; set;}
}";

        var expected = @"
using Dolittle.SDK.Events;

[EventType(alias: ""Foo"", eventTypeId: ""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeEvent
{
    public string Name {get; set;}
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";

        var diagnosticResult = Diagnostic(DescriptorRules.InvalidIdentity)
            .WithSpan(4, 2, 4, 42)
            .WithArguments("EventType", "eventTypeId", "");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task FixesEventHandlers()
    {
        var test = @"
using Dolittle.SDK.Events.Handling;

[EventHandler(alias: ""Foo"", eventHandlerId: ""invalid-id"")]
class SomeHandler
{
}";

        var expected = @"
using Dolittle.SDK.Events.Handling;

[EventHandler(alias: ""Foo"", eventHandlerId: ""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeHandler
{
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";

        var diagnosticResult = Diagnostic(DescriptorRules.InvalidIdentity)
            .WithSpan(4, 2, 4, 58)
            .WithArguments("EventHandler", "eventHandlerId", "invalid-id");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }

    [Fact]
    public async Task FixesProjections()
    {
        var test = @"
using Dolittle.SDK.Projections;

[Projection("""")]
class SomeProjection: ReadModel
{
}";

        var expected = @"
using Dolittle.SDK.Projections;

[Projection(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeProjection: ReadModel
{
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";

        var diagnosticResult = Diagnostic(DescriptorRules.InvalidIdentity)
            .WithSpan(4, 2, 4, 16)
            .WithArguments("Projection", "projectionId", "");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }

    [Fact]
    public async Task FixesProjectionsWithNamedParameters()
    {
        var test = @"
using Dolittle.SDK.Projections;

[Projection(alias: ""Foo"", projectionId: ""invalid-id"")]
class SomeProjection: ReadModel
{
}";

        var expected = @"
using Dolittle.SDK.Projections;

[Projection(alias: ""Foo"", projectionId: ""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeProjection: ReadModel
{
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";

        var diagnosticResult = Diagnostic(DescriptorRules.InvalidIdentity)
            .WithSpan(4, 2, 4, 54)
            .WithArguments("Projection", "projectionId", "invalid-id");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }

    [Fact]
    public async Task FixesAggregates()
    {
        var test = @"
using Dolittle.SDK.Aggregates;

[AggregateRoot(alias: ""Foo"", id: ""invalid-id"")]
class SomeAggregate: AggregateRoot
{
}";

        var expected = @"
using Dolittle.SDK.Aggregates;

[AggregateRoot(alias: ""Foo"", id: ""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
class SomeAggregate: AggregateRoot
{
}";
        IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";

        var diagnosticResult = Diagnostic(DescriptorRules.InvalidIdentity)
            .WithSpan(4, 2, 4, 47)
            .WithArguments("AggregateRoot", "id", "invalid-id");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task WhenFixingRedactionEvents()
    {
        var test = @"
[Dolittle.SDK.Events.EventType(""e8879da9-fd28-4c78-b9cc-1381a09c3e79"")]
class SomeEvent
{
    public string Name {get; set;}
};

[Dolittle.SDK.Events.EventType(""hello-0000-da7a-aaaa-fbc6ec3c0ea6"")]
class RedactionEvent: Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent<SomeEvent>
{
}";

        var expected = @"
[Dolittle.SDK.Events.EventType(""e8879da9-fd28-4c78-b9cc-1381a09c3e79"")]
class SomeEvent
{
    public string Name {get; set;}
};

[Dolittle.SDK.Events.EventType(""de1e7e17-bad5-da7a-8a81-6816d3877f81"")]
class RedactionEvent: Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent<SomeEvent>
{
}";
        IdentityGenerator.OverrideRedaction = "de1e7e17-bad5-da7a-8a81-6816d3877f81";

        var diagnosticResult = Diagnostic(DescriptorRules.IncorrectRedactedEventTypePrefix)
            .WithSpan(8, 2, 8, 68)
            .WithArguments("Dolittle.SDK.Events.EventType", "eventTypeId", "hello-0000-da7a-aaaa-fbc6ec3c0ea6");
        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
}

