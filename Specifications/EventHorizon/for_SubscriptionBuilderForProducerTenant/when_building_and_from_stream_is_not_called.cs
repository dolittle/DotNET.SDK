// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionBuilderForProducerTenant
{
    public class when_building_and_from_stream_is_not_called
    {
        static Mock<IEventHorizons> event_horizons;
        static SubscriptionBuilderForProducerTenant builder;

        static TenantId consumer_tenant;
        static MicroserviceId producer_microservice;
        static TenantId producer_tenant;

        static Exception exception;

        Establish context = () =>
        {
            event_horizons = new Mock<IEventHorizons>();
            event_horizons.SetupGet(_ => _.Responses).Returns(Mock.Of<IObservable<SubscribeResponse>>());

            consumer_tenant = "efd27e51-4793-43e0-93e8-ea7acbd404dc";
            producer_microservice = "dd5793cc-b8e5-4c91-891f-cd5ae6af82a3";
            producer_tenant = "c30805f3-4431-45a6-8047-2c1d1b40edcd";
            builder = new SubscriptionBuilderForProducerTenant(consumer_tenant, producer_microservice, producer_tenant);
        };

        Because of = () => exception = Catch.Exception(() => builder.BuildAndSubscribe(event_horizons.Object, CancellationToken.None));

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<SubscriptionDefinitionIncomplete>();
    }
}
