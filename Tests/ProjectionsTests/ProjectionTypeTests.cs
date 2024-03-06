using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Internal;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace ProjectionsTests;

public class ProjectionTypeTests
{
    const string ProjectionKey = "foo";
    static readonly ProjectionContext _projectionContext = new(true, ProjectionKey, TestEventContexts.ForType<SomeEvent>());

    [Fact]
    public void CanGetProjectionOnMethods()
    {
        var methodInfo = ProjectionType<TestProjection>.MethodsPerEventType;

        methodInfo.Should().NotBeNull();
        methodInfo.Keys.Should().BeEquivalentTo(new[]
        {
            typeof(SomeEvent),
            typeof(SomeOtherEvent),
            typeof(DeleteEvent)
        });
    }

    [Fact]
    public void CanMutateProjections()
    {
        var testProjection = new TestProjection
        {
            Id = ProjectionKey,
        };
        var someEvent = new SomeEvent
        {
            Thing = "foo"
        };

        var result = ProjectionType<TestProjection>.Apply(testProjection, someEvent, _projectionContext);

        using var _ = new AssertionScope();

        result.Type.Should().Be(ProjectionResultType.Replace);
        result.ReadModel.Should()
            .BeEquivalentTo(new TestProjection
            {
                Id = ProjectionKey,
                UpdateCount = 1,
                Content = "foo"
            });
    }

    [Fact]
    public void CanMutateProjectionsWithEventContextSignature()
    {
                var testProjection = new TestProjection
        {
            Id = ProjectionKey,
        };
        var someEvent = new SomeOtherEvent()
        {
            SomeNumber = 42
        };

        var result = testProjection.Apply(someEvent, _projectionContext);

        using var _ = new AssertionScope();

        result.Type.Should().Be(ProjectionResultType.Replace);
        result.ReadModel.Should()
            .BeEquivalentTo(new TestProjection
            {
                Id = ProjectionKey,
                UpdateCount = 1,
                TheNumber = 42
            });
    }

    [Fact]
    public void CanDeleteProjections()
    {
        var testProjection = new TestProjection
        {
            Id = ProjectionKey,
        };
        
        var result = testProjection.Apply(new SomeEvent { Thing = "foo" }, _projectionContext);

        var before = result.ReadModel!;

        var deleteResult = before.Apply(new DeleteEvent { Reason = "Just because" }, _projectionContext);

        using var _ = new AssertionScope();

        deleteResult.Type.Should().Be(ProjectionResultType.Delete);
        deleteResult.ReadModel.Should().BeNull();
    }
}
