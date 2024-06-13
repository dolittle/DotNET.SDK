// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.CodeFixes;

public class ProjectionMutationEventTimeCodeFixProviderTests : CodeFixProviderTests<ProjectionsAnalyzer, ProjectionMutationEventTimeCodeFixProvider>
{
    [Fact]
    public async Task ShouldFixDateTimeNow()
    {
        var test = @"
using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
public record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void On(NameUpdated evt) {
        Name = evt.Name;
        UpdatedAt = DateTime.Now;
    }
}";

        var expected = @"using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
public record NameUpdated(string Name);
[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection : ReadModel
{
    public string Name { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void On(NameUpdated evt, EventContext ctx)
    {
        Name = evt.Name;
        UpdatedAt = ctx.Occurred.DateTime;
    }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Projection.MutationUsedCurrentTime)
            .WithSpan(17, 21, 17, 33)
            .WithArguments("DateTime.Now");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task ShouldFixDateTimeOffsetNow()
    {
        var test = @"
using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
public record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void On(NameUpdated evt) {
        Name = evt.Name;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}";

        var expected = @"using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
public record NameUpdated(string Name);
[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection : ReadModel
{
    public string Name { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void On(NameUpdated evt, EventContext ctx)
    {
        Name = evt.Name;
        UpdatedAt = ctx.Occurred;
    }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Projection.MutationUsedCurrentTime)
            .WithSpan(17, 21, 17, 42)
            .WithArguments("DateTimeOffset.UtcNow");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task ShouldFixDateTimeOffsetNowWithPreExistingEventContext()
    {
        var test = @"
using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
public record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void On(NameUpdated evt, EventContext context) {
        Name = evt.Name;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}";

        var expected = @"using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
public record NameUpdated(string Name);
[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection : ReadModel
{
    public string Name { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void On(NameUpdated evt, EventContext context)
    {
        Name = evt.Name;
        UpdatedAt = context.Occurred;
    }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Projection.MutationUsedCurrentTime)
            .WithSpan(17, 21, 17, 42)
            .WithArguments("DateTimeOffset.UtcNow");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
}
