using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Xunit;

namespace ProjectionsTests;

public class ProjectionTypeTests : ProjectionTests<TestProjection>
{
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

        ReadModel(ProjectionKey).Should().BeEquivalentTo(new TestProjection
        {
            Id = ProjectionKey,
            Content = "foo",
            LastUpdated = when,
            UpdateCount = 2,
            TheNumber = 10
        });
    }

    [Fact]
    public void CanMutateProjectionsWithEventContextSignature()
    {
        WithEvent(ProjectionKey, new SomeOtherEvent()
        {
            SomeNumber = 42
        });

        ReadModel(ProjectionKey)!.TheNumber.Should().Be(42);
    }
    
    [Fact]
    public void CanDeleteProjections()
    {
        WithEvent(ProjectionKey, new SomeEvent
        {
            Thing = "foo"
        });
        WithEvent(ProjectionKey, new DeleteEvent { Reason = "Just because" });
        
        ReadModel(ProjectionKey).Should().BeNull();
    }
}
