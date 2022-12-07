// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Analyzers.Suppressors;

namespace Dolittle.SDK.Analyzers.Suppressor;

public class AggregateMutationNotUsedSuppressorTests : DefaultAnalysisSuppressorTest<AggregateMutationNotUsedSuppressor>
{
    [Fact]
    public async Task ShouldSuppressUnusedOnMethod()
    {
        var test = @"
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;


[EventType(""5dc02e84-c6fc-4e1b-997c-ec33d0048a3b"")]
record NameUpdated(string Name);

[AggregateRoot(""10ef9f40-3e61-444a-9601-f521be2d547e"")]
class SomeAggregate: AggregateRoot
{
    public string Name {get; set;}

    public void UpdateName(string name)
    {
        Apply(new NameUpdated(name));
    }

    private void on(NameUpdated @event)
    {
        Name = @event.Name;
    }
}";
        await VerifyAnalyzerFindsNothingAsync(test);
    }
}
