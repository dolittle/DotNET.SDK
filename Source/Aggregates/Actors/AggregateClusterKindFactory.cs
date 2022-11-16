// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates.Internal;
using Microsoft.Extensions.DependencyInjection;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Aggregates.Actors;

static class AggregateClusterKindFactory
{
    public static ClusterKind CreateKind<TAggregate>(IServiceProvider serviceProvider) where TAggregate : AggregateRoot
    {
        var aggregateRootType = AggregateRootMetadata<TAggregate>.AggregateRootType ?? throw new MissingAggregateRootAttribute(typeof(TAggregate));

        return new ClusterKind(aggregateRootType.Id.Value.ToString(),
            Props.FromProducer(() => ActivatorUtilities.CreateInstance<AggregateActor<TAggregate>>(serviceProvider)));
    }
}
