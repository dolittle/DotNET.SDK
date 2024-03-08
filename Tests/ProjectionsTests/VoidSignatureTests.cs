// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Types;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Xunit;

namespace Dolittle.SDK.Projections;

public class VoidSignatureTests : ProjectionTests<VoidSignaturesProjection>
{
    const string EventSourceId = "bob";
    const string ExpectedContent = "The content";

    [Fact]
    public void ShouldProjectOnVoidProjectionContext()
    {
        WithEvent(EventSourceId, new VoidProjection(ExpectedContent));

        ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }
    
    [Fact]
    public void ShouldProjectOnVoidEventContext()
    {
        WithEvent(EventSourceId, new VoidEvent(ExpectedContent));

        ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }
    
    [Fact]
    public void ShouldProjectOnVoidEvent()
    {
        WithEvent(EventSourceId, new VoidEventContext(ExpectedContent));

        ReadModel(EventSourceId)!.Content.Should().Be(ExpectedContent);
    }
}
