// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Core;
using Dolittle.SDK.ProjectionTests.InvalidProjections;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Xunit;

namespace Dolittle.SDK.ProjectionTests;

public class InvalidProjectionTests
{
    [Fact]
    public void ShouldThrowOnDuplicateHandlers()
    {
        var get = () =>
        {
            var projection = ProjectionFixture<DuplicateHandlers>.Projection;
        };

        get.Invoking(it => it()).Should()
            .Throw<TypeInitializationException>()
            .WithInnerException(typeof(ArgumentException))
            .WithMessage("""
                         Multiple handlers for ProjectionsTests.SomeEvent
                         """);
    }

    [Fact]
    public void ShouldThrowOnMissingAttribute()
    {
        var get = () =>
        {
            var projection = ProjectionFixture<MissingAttributeProjection>.Projection;
        };

        get.Invoking(it => it()).Should()
            .Throw<TypeInitializationException>()
            .WithInnerException(typeof(MissingProjectionAttribute))
            .WithMessage($"""
                          Missing [Projection] attribute on MissingAttributeProjection
                          """);
    }
}
