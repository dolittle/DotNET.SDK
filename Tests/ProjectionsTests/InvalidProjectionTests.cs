// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Internal;
using FluentAssertions;
using Xunit;

namespace ProjectionsTests;

public class InvalidProjectionTests
{
    [Fact]
    public void ShouldThrowOnInvalidProjections()
    {
        var create = () =>
        {
            var id = ProjectionType<InvalidProjection>.ProjectionModelId;
        };

        create.Invoking(it => it()).Should()
            .Throw<TypeInitializationException>()
            .WithInnerException(typeof(DuplicateHandlerForEventType));
    }
}
