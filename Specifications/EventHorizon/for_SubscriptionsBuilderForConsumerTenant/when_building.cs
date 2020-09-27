// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionsBuilderForConsumerTenant
{
    public class when_building
    {
        static Mock<IEventHorizons> event_horizons;
        static SubscriptionsBuilderForConsumerTenant builder;

        static TenantId consumer_tenant;
        static MicroserviceId first_producer_microservice;
        static MicroserviceId second_producer_microservice;

        Establish context = () =>
        {
            event_horizons = new Mock<IEventHorizons>();
            event_horizons.SetupGet(_ => _.Responses).Returns(Mock.Of<IObservable<SubscribeResponse>>());

            consumer_tenant = "8651e882-736e-4171-b13f-8a3a1ab76b7c";
            builder = new SubscriptionsBuilderForConsumerTenant(consumer_tenant);

            first_producer_microservice = "db8396e6-2df7-4f3a-b6d1-9a72c3981f3e";
            builder.FromMicroservice(first_producer_microservice, fromMicroservice =>
            {
                fromMicroservice
                    .FromTenant("b194dc51-3f84-44fe-9e1c-c04634e14064")
                    .FromStream("9ef561dd-a1ac-4e30-baae-46674f669eae")
                    .FromPartition("aa440723-bc9a-46c3-8777-fadbc9f472f6")
                    .ToScope("3d87171e-8b04-4cb5-92f2-966e5c7a5371");
            });

            second_producer_microservice = "28c18822-b23d-4adc-9974-40d10e138972";
            builder.FromMicroservice(second_producer_microservice, fromMicroservice =>
            {
                fromMicroservice
                    .FromTenant("b194dc51-3f84-44fe-9e1c-c04634e14064")
                    .FromStream("9ef561dd-a1ac-4e30-baae-46674f669eae")
                    .FromPartition("aa440723-bc9a-46c3-8777-fadbc9f472f6")
                    .ToScope("3d87171e-8b04-4cb5-92f2-966e5c7a5371");
            });
        };

        Because of = () => builder.BuildAndSubscribe(event_horizons.Object, CancellationToken.None);

        It should_call_event_horizons_subscribe_for_the_first_microservice = () => event_horizons.Verify(_ => _.Subscribe(Moq.It.Is<Subscription>(
            _ => _.ConsumerTenant == consumer_tenant
                && _.ProducerMicroservice == first_producer_microservice)));

        It should_call_event_horizons_subscribe_for_the_second_microservice = () => event_horizons.Verify(_ => _.Subscribe(Moq.It.Is<Subscription>(
            _ => _.ConsumerTenant == consumer_tenant
                && _.ProducerMicroservice == second_producer_microservice)));
    }
}
