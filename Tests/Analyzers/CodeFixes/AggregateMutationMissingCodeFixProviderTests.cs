// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Analyzers.CodeFixes;

public class AggregateMutationMissingCodeFixProviderTests : CodeFixProviderTests<AggregateAnalyzer, AggregateMutationCodeFixProvider>
{
    [Fact]
    public async Task ShouldFixMissingMutationFromConstructor()
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
}";

        var expected = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate : AggregateRoot
{
    public string Name { get; set; }

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void On(NameUpdated @event) => throw new System.NotImplementedException();
}";
        var diagnosticResult = Diagnostic(DescriptorRules.Aggregate.MissingMutation)
            .WithSpan(15, 9, 15, 37)
            .WithArguments("NameUpdated");

        await VerifyCodeFixAsync(test, expected, diagnosticResult);
    }
    
//     [Fact]
//     public async Task ShouldCleanupRedundantNamespaces()
//     {
//         var test = @"
// using System;
// using Dolittle.SDK.Aggregates;
// using Dolittle.SDK.Events;
//
// [EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
// record NameUpdated(string Name);
//
// [AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
// class SomeAggregate: AggregateRoot
// {
//     public string Name { get; set; }
//
//     public void UpdateName(string name)
//     {
//         Apply(new NameUpdated(name));
//     }
// }";
//
//         var expected = @"
// using System;
// using Dolittle.SDK.Aggregates;
// using Dolittle.SDK.Events;
//
// [EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
// record NameUpdated(string Name);
//
// [AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
// class SomeAggregate : AggregateRoot
// {
//     public string Name { get; set; }
//
//     public void UpdateName(string name)
//     {
//         Apply(new NameUpdated(name));
//     }
//
//     private void On(NameUpdated @event) => throw new NotImplementedException();
// }";
//         var diagnosticResult = Diagnostic(DescriptorRules.Aggregate.MissingMutation)
//             .WithSpan(16, 9, 16, 37)
//             .WithArguments("NameUpdated");
//
//         await VerifyCodeFixAsync(test, expected, diagnosticResult);
//     }
}
