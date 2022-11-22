// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates.Actors;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;
using FluentAssertions;

namespace Aggregates;

public class ClusterIdentityMapperTests
{
    [Theory]
    [InlineData("1ea76ca4-a251-4e89-a409-bbc03170637a", "c77bca02-c95c-4171-bd14-86236fe32118")]
    [InlineData("1ea76ca4-a251-4e89-a409-bbc03170637b", "foo")]
    [InlineData("1ea76ca4-a251-4e89-a409-bbc03170637c", "foo/bar")]
    [InlineData("1ea76ca4-a251-4e89-a409-bbc03170637d", "foo:bar")]
    [InlineData("1ea76ca4-a251-4e89-a409-bbc03170637d", "foo:bar:baz")]
    [InlineData("1ea76ca4-a251-4e89-a409-bbc03170637d", ":foo:bar:baz-waz")]
    [InlineData("1ea76ca4-a251-4e89-a409-bbc03170637d", "123_foo:bar:baz-waz")]
    public void CanMapIdentityCorrectly(string tenant, string eventSource)
    {
        // bug in xunit theories parameter deserialization: https://github.com/xunit/xunit/issues/1742
        TenantId tenantId = tenant;
        EventSourceId eventSourceId = eventSource;

        var clusterIdentity = ClusterIdentityMapper.GetClusterIdentity<TestAggregate>(tenantId, eventSourceId);

        var (outputTenantId, outputEventSourceId) = ClusterIdentityMapper.GetTenantAndEventSourceId(clusterIdentity);

        outputTenantId.Should().Be(tenantId);
        outputEventSourceId.Should().Be(eventSourceId);
    }
}
