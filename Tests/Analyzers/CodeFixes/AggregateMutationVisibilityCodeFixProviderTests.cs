// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.CodeFixes;

public class AggregateMutationVisibilityCodeFixProviderTests : CodeFixProviderTests<AggregateAnalyzer, MethodVisibilityCodeFixProvider>
{
    [Fact]
    public async Task ShouldMakePublicPrivate()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    public void On(NameUpdated evt) { }
}";

        var expected = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On(NameUpdated evt) { }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Aggregate.MutationShouldBePrivate)
            .WithSpan(18, 5, 18, 40)
            .WithArguments("SomeAggregate.On(NameUpdated)");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task ShouldMakeInternalPrivate()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    internal void On(NameUpdated evt) { }
}";

        var expected = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On(NameUpdated evt) { }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Aggregate.MutationShouldBePrivate)
            .WithSpan(18, 5, 18, 42)
            .WithArguments("SomeAggregate.On(NameUpdated)");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
    [Fact]
    public async Task ShouldMakeProtectedPrivate()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    protected void On(NameUpdated evt) { }
}";

        var expected = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    void On(NameUpdated evt) { }
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Aggregate.MutationShouldBePrivate)
            .WithSpan(18, 5, 18, 43)
            .WithArguments("SomeAggregate.On(NameUpdated)");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
}
