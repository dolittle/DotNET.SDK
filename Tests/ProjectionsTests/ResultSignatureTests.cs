// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Types;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Xunit;

namespace Dolittle.SDK.Projections;

public class ResultSignatureTests : ProjectionTests<ResultSignaturesProjection>
{
    const string EventSourceId = "bob";
    const string ExpectedContent = "The content";

    [Fact]
    public void ShouldProjectOnResultProjectionContext()
    {
        WithEvent(EventSourceId, new ResultProjection(ExpectedContent));

        AssertThat.ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }

    [Fact]
    public void ShouldProjectOnResultEventContext()
    {
        WithEvent(EventSourceId, new ResultEvent(ExpectedContent));

        AssertThat.ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }

    [Fact]
    public void ShouldProjectOnResultEvent()
    {
        WithEvent(EventSourceId, new ResultEventContext(ExpectedContent));

        AssertThat.ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }

    [Fact]
    public void ShouldProjectOnResultDelete()
    {
        WithEvent(EventSourceId, new ResultEventContext(ExpectedContent));
        WithEvent(EventSourceId, new ResultDelete());

        AssertThat.ReadModelDoesNotExist(EventSourceId);
    }
}
