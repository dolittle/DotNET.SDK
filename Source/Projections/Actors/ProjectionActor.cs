// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Projections.Internal;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Projections.Actors;

public class ProjectionActor<TProjection> : IActor
    where TProjection : ProjectionBase, new()
{
    public static string Kind => $"proj_{ProjectionType<TProjection>.ProjectionModelId!.Id.Value:N}";

    readonly GetServiceProviderForTenant _providerForTenant;
    readonly ILogger<ProjectionActor<TProjection>> _logger;

    public ProjectionActor(GetServiceProviderForTenant providerForTenant, ILogger<ProjectionActor<TProjection>> logger, TimeSpan idleUnloadTimeout)
    {
        _providerForTenant = providerForTenant;
        _logger = logger;
    }

    public Task ReceiveAsync(IContext context)
    {
        try
        {
            switch (context.Message)
            {
                case Started:
                    return Init(context.ClusterIdentity()!.Identity);
                default:
                    return Task.CompletedTask;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing {Message}", context.Message);
            context.Stop(context.Self);
            return Task.CompletedTask;
        }
    }

    Task Init(string id)
    {
        return Task.CompletedTask;
    }
}
