// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Types;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Xunit;

namespace Dolittle.SDK.Projections;

public class AggregateProjectionTests : ProjectionTests<AggregateProjection>
{
    static readonly EventSourceId _eventSourceId = "foo";

    [Fact]
    public void ShouldUpdateProjectionOnAggregateChanges()
    {
        WhenAggregateMutated<TestAggregate>(_eventSourceId, agg => agg.Rename("Bob"));

        AssertThat.HasReadModel(_eventSourceId.Value)
            .Where(
                it => it.Name.Should().Be("Bob"),
                it => it.TimesChanged.Should().Be(1));
    }

    [Fact]
    public void ShouldUpdateProjectionOnAggregateChangesAgain()
    {
        WhenAggregateMutated<TestAggregate>(_eventSourceId, agg =>
        {
            agg.Rename("Bob");
            agg.Rename("Bobby");
        });

        var projection = AssertThat.ReadModel(_eventSourceId.Value);
        projection.Name.Should().Be("Bobby");
        projection.TimesChanged.Should().Be(2);
    }

    [Fact]
    public void ShouldUpdateProjectionOnAggregateChangesAgainAndAgain()
    {
        WhenAggregateMutated<TestAggregate>(_eventSourceId, agg =>
        {
            agg.Rename("Bob");
            agg.Rename("Bobby");
        });

        WhenAggregateMutated<TestAggregate>(_eventSourceId, agg =>
        {
            agg.Rename("Bobby");
            agg.Rename("Bob");
        });

        var projection = AssertThat.ReadModel(_eventSourceId.Value);
        projection.Name.Should().Be("Bob");
        projection.TimesChanged.Should().Be(3);
    }
}
