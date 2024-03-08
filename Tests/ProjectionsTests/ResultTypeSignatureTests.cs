// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Types;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Xunit;

namespace Dolittle.SDK.Projections;

public class ResultTypeSignatureTests : ProjectionTests<ResultTypeSignaturesProjection>
{
    const string EventSourceId = "bob";
    const string ExpectedContent = "The content";

    [Fact]
    public void ShouldProjectOnResultTypeProjectionContext()
    {
        WithEvent(EventSourceId, new ResultTypeProjection(ExpectedContent));

        ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }

    [Fact]
    public void ShouldProjectOnResultTypeEventContext()
    {
        WithEvent(EventSourceId, new ResultTypeEvent(ExpectedContent));

        ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }

    [Fact]
    public void ShouldProjectOnResultTypeEvent()
    {
        WithEvent(EventSourceId, new ResultTypeEventContext(ExpectedContent));

        ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }

    [Fact]
    public void ShouldProjectOnResultTypeDelete()
    {
        WithEvent(EventSourceId, new ResultTypeEventContext(ExpectedContent));
        WithEvent(EventSourceId, new ResultTypeDelete());

        ReadModelShouldBeDeleted(EventSourceId);
    }
}
