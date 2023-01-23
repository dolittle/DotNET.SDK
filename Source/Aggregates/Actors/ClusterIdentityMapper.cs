// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;
using Proto.Cluster;

namespace Dolittle.SDK.Aggregates.Actors;

static class ClusterIdentityMapper
{
    public static ClusterIdentity GetClusterIdentity<TAggregate>(TenantId tenantId, EventSourceId eventSourceId) where TAggregate : AggregateRoot =>
        ClusterIdentity.Create($"{tenantId}:{eventSourceId}", AggregateRootMetadata<TAggregate>.GetAggregateRootId().Value.ToString());

    public static (TenantId, EventSourceId) GetTenantAndEventSourceId(ClusterIdentity clusterIdentity)
    {
        if (clusterIdentity is null)
        {
            throw new ArgumentNullException(nameof(clusterIdentity));
        }
        var separator = clusterIdentity.Identity.IndexOf(":", StringComparison.Ordinal);
        
        if(separator == -1)
        {
            throw new ArgumentException("ClusterIdentity is not in the correct format", nameof(clusterIdentity));
        }

        TenantId tenantId = clusterIdentity.Identity[..separator];
        EventSourceId eventSourceId = clusterIdentity.Identity[(separator + 1)..];

        return (tenantId, eventSourceId);
    }
}
