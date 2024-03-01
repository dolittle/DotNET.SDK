﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Internal;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Projections.Actors;

static class ProjectionClusterKindFactory
{
    public static ClusterKind CreateKind(IServiceProvider serviceProvider, Type projectionType)
    {
        var createKind = typeof(ProjectionClusterKindFactory<>).MakeGenericType(projectionType).GetMethod(nameof(CreateKind));
        return (ClusterKind)createKind!.Invoke(null, [serviceProvider])!;
    }
}

static class ProjectionClusterKindFactory<TProjection> where TProjection : ProjectionBase, new()
{
    // ReSharper disable once UnusedMember.Global - Called by reflection
    public static ClusterKind CreateKind(IServiceProvider serviceProvider)
    {
        if (ProjectionType<TProjection>.ProjectionModelId is null)
        {
            throw new ArgumentException($"Projection type {typeof(TProjection).FullName} is missing the Projection attribute");
        }

        var providerForTenant = serviceProvider.GetRequiredService<GetServiceProviderForTenant>();
        var logger = serviceProvider.GetRequiredService<ILogger<ProjectionActor<TProjection>>>();
        var idleUnloadTimeout = TimeSpan.FromSeconds(20); // TODO: make timeouts configurable

        return new ClusterKind(ProjectionActor<TProjection>.Kind,
            Props.FromProducer(() => new ProjectionActor<TProjection>(providerForTenant, logger, idleUnloadTimeout))
                .WithClusterRequestDeduplication(TimeSpan.FromMinutes(5)));
    }
}
