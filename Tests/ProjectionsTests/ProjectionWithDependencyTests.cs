// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Types;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dolittle.SDK.Projections;

public class ProjectionWithDependencyTests : ProjectionTests<ReadModelWithDependency>
{
    static readonly List<NameChanged> _nameChangedEvents = new();

    public ProjectionWithDependencyTests() : base(ConfigureServices)
    {
        _nameChangedEvents.Clear();
    }

    static void ConfigureServices(IServiceCollection services)
    {
        // This configures the TestDependency that is resolved by the projection DI
        services.AddSingleton<TestDependency>(evt =>
        {
            _nameChangedEvents.Add(evt);
        });
    }

    static readonly EventSourceId _eventSourceId = "foo";

    [Fact]
    public void ShouldUpdateProjectionOnAggregateChanges()
    {
        WhenAggregateMutated<TestAggregate>(_eventSourceId, agg => agg.Rename("Bob"));

        AssertThat.HasReadModel(_eventSourceId.Value)
            .AndThat(
                it => it.Name.Should().Be("Bob"),
                it => it.TimesChanged.Should().Be(1));
        
        _nameChangedEvents.Should().HaveCount(1);
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
        _nameChangedEvents.Should().HaveCount(2);
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
        _nameChangedEvents.Should().HaveCount(3);
    }
}
