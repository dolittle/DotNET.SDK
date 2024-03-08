// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;
using Proto.Cluster;

namespace Dolittle.SDK.Projections.Actors;

static class ClusterIdentityMapper
{
    public static ClusterIdentity GetClusterIdentity(TenantId tenantId, Key projectionKey, string kind) =>
        ClusterIdentity.Create($"{tenantId}:{projectionKey}", kind);

    public static (TenantId, Key) GetTenantAndKey(ClusterIdentity clusterIdentity)
    {
        ArgumentNullException.ThrowIfNull(clusterIdentity);
        var separator = clusterIdentity.Identity.IndexOf(':');
        
        if(separator == -1)
        {
            throw new ArgumentException("ClusterIdentity is not in the correct format", nameof(clusterIdentity));
        }

        TenantId tenantId = clusterIdentity.Identity[..separator];
        Key projectionKey = clusterIdentity.Identity[(separator + 1)..];

        return (tenantId, projectionKey);
    }
}
