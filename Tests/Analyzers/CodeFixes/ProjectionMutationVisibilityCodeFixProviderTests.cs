// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.CodeFixes;

public class ProjectionMutationVisibilityCodeFixProviderTests : CodeFixProviderTests<ProjectionsAnalyzer, MethodVisibilityCodeFixProvider>
{


    [Fact]
    public async Task ShouldFixVisibilityWhenPrivate()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }

    private void On(NameUpdated evt) { }
}";

        var expected = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }

    public void On(NameUpdated evt) { }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Projection.InvalidOnMethodVisibility)
            .WithSpan(13, 5, 13, 41)
            .WithArguments("SomeProjection.On(NameUpdated)");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task ShouldFixVisibilityWhenInternal()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }

    internal void On(NameUpdated evt) { }
}";

        var expected = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }

    public void On(NameUpdated evt) { }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Projection.InvalidOnMethodVisibility)
            .WithSpan(13, 5, 13, 42)
            .WithArguments("SomeProjection.On(NameUpdated)");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task ShouldFixVisibilityWhenProtected()
    {
        var test = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }

    protected void On(NameUpdated evt) { }
}";

        var expected = @"
using Dolittle.SDK.Projections;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[Projection(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeProjection: ReadModel
{
    public string Name { get; set; }

    public void On(NameUpdated evt) { }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Projection.InvalidOnMethodVisibility)
            .WithSpan(13, 5, 13, 43)
            .WithArguments("SomeProjection.On(NameUpdated)");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
}
