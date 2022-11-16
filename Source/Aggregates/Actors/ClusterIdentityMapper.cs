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
        if (clusterIdentity is null) throw new ArgumentNullException(nameof(clusterIdentity));
        var parts = clusterIdentity.Identity.Split(':');
        if (parts.Length != 2) throw new ArgumentException($"The cluster identity '{clusterIdentity.Identity}' is not a valid aggregate identity");
        TenantId tenantId = parts[0];
        EventSourceId eventSourceId = parts[1];
        return (tenantId, eventSourceId);
    }
}
