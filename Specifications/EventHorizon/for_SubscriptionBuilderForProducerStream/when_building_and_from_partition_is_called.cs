// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionBuilderForProducerStream;

public class when_building_and_from_partition_is_called
{
    static Mock<IEventHorizons> event_horizons;
    static SubscriptionBuilderForProducerStream builder;

    static TenantId consumer_tenant;
    static MicroserviceId producer_microservice;
    static TenantId producer_tenant;
    static StreamId producer_stream;
    static PartitionId producer_partition;

    Establish context = () =>
    {
        event_horizons = new Mock<IEventHorizons>();
        event_horizons.SetupGet(_ => _.Responses).Returns(Mock.Of<IObservable<SubscribeResponse>>());

        consumer_tenant = "efd27e51-4793-43e0-93e8-ea7acbd404dc";
        producer_microservice = "dd5793cc-b8e5-4c91-891f-cd5ae6af82a3";
        producer_tenant = "c30805f3-4431-45a6-8047-2c1d1b40edcd";
        producer_stream = "e617fc5a-cec8-41ee-b9ad-361f8cfe7641";
        builder = new SubscriptionBuilderForProducerStream(consumer_tenant, producer_microservice, producer_tenant, producer_stream);

        producer_partition = "e9395405-0906-4baa-a7da-e9641c548572";
        builder
            .FromProducerPartition(producer_partition)
            .ToScope("3d87171e-8b04-4cb5-92f2-966e5c7a5371");
    };

    Because of = () => builder.BuildAndSubscribe(event_horizons.Object, CancellationToken.None);

    It should_call_event_horizons_subscribe = () => event_horizons.Verify(_ => _.Subscribe(Moq.It.Is<Subscription>(
        _ => _.ConsumerTenant == consumer_tenant
            && _.ProducerMicroservice == producer_microservice
            && _.ProducerTenant == producer_tenant
            && _.ProducerStream == producer_stream
            && _.ProducerPartition == producer_partition)));
}