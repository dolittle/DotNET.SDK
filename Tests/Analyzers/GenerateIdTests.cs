// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;

namespace Dolittle.SDK.Analyzers;

public class GenerateIdTests
{
    [Fact]
    public void WhenGeneratingId()
    {
        var id = IdentityGenerator.GenerateRedactionId();

        id.Should().NotBeEmpty();
        Guid.TryParse(id, out _).Should().BeTrue("Should be a valid Guid");
    }
    
    [Fact]
    public void WhenGeneratingRedactionId()
    {
        var id = IdentityGenerator.GenerateRedactionId();

        id.Should().NotBeEmpty();
        Guid.TryParse(id, out _).Should().BeTrue("Should be a valid Guid");
        id.Should().StartWith("de1e7e17-bad5-da7a-", "Should start with the correct prefix");
    }
}
