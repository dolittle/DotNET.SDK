using Dolittle.SDK.Projections.Internal;
using Dolittle.SDK.Projections.Types;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Xunit;

namespace Dolittle.SDK.Projections;

public class ProjectionTypeTests : ProjectionTests<TestProjection>
{
    [Fact]
    public void CanParseUnloadTimeouts()
    {
        ProjectionType<TestProjection>.IdleUnloadTimeout.Should().Be(TimeSpan.FromMinutes(1));
    }

    const string ProjectionKey = "foo";

    [Fact]
    public void CanMutateProjections()
    {
        var when = DateTimeOffset.Now;

        WithEvent(ProjectionKey, new SomeEvent
        {
            Thing = "foo"
        });
        WithEvent(ProjectionKey, new SomeOtherEvent
        {
            SomeNumber = 10
        }, when);

        AssertThat.ReadModel(ProjectionKey).Should().BeEquivalentTo(new TestProjection
        {
            Id = ProjectionKey,
            Content = "foo",
            LastUpdated = when,
            UpdateCount = 2,
            TheNumber = 10,
            EventOffset = 1
        });
    }

    [Fact]
    public void CanMutateProjectionsWithEventContextSignature()
    {
        WithEvent(ProjectionKey, new SomeOtherEvent()
        {
            SomeNumber = 42
        });

        AssertThat.ReadModel(ProjectionKey)!.TheNumber.Should().Be(42);
    }

    [Fact]
    public void CanDeleteProjections()
    {
        WithEvent(ProjectionKey, new SomeEvent
        {
            Thing = "foo"
        });
        WithEvent(ProjectionKey, new DeleteEvent { Reason = "Just because" });

        AssertThat.ReadModelDoesNotExist(ProjectionKey);
    }
}
