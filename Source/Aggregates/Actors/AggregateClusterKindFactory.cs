// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Aggregates.Actors;

static class AggregateClusterKindFactory
{
    public static ClusterKind CreateKind(IServiceProvider serviceProvider, Type aggregateRootType)
    {
        var createKind = typeof(AggregateClusterKindFactory<>).MakeGenericType(aggregateRootType).GetMethod(nameof(CreateKind));
        return (ClusterKind)createKind!.Invoke(null, new object[] { serviceProvider })!;
    }
}

static class AggregateClusterKindFactory<TAggregate> where TAggregate : AggregateRoot
{
    // ReSharper disable once UnusedMember.Global - Called by reflection
    public static ClusterKind CreateKind(IServiceProvider serviceProvider)
    {
        var aggregateRootType = AggregateRootMetadata<TAggregate>.AggregateRootType ?? throw new MissingAggregateRootAttribute(typeof(TAggregate));
        var providerForTenant = serviceProvider.GetRequiredService<GetServiceProviderForTenant>();
        var logger = serviceProvider.GetRequiredService<ILogger<AggregateActor<TAggregate>>>();
        var idleUnloadTimeout = serviceProvider.GetRequiredService<AggregateUnloadTimeout>()();

        return new ClusterKind(aggregateRootType.Id.Value.ToString(),
            Props.FromProducer(() => new AggregateActor<TAggregate>(providerForTenant, logger, idleUnloadTimeout)));
    }
}
