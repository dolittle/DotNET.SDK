// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionsBuilderForProducerMicroservice
{
    public class when_building_and_from_tenant_is_not_called
    {
        static Mock<IEventHorizons> event_horizons;
        static SubscriptionsBuilderForProducerMicroservice builder;

        static TenantId consumer_tenant;
        static MicroserviceId producer_microservice;

        static Exception exception;

        Establish context = () =>
        {
            event_horizons = new Mock<IEventHorizons>();
            event_horizons.SetupGet(_ => _.Responses).Returns(Mock.Of<IObservable<SubscribeResponse>>());

            consumer_tenant = "efd27e51-4793-43e0-93e8-ea7acbd404dc";
            producer_microservice = "dd5793cc-b8e5-4c91-891f-cd5ae6af82a3";
            builder = new SubscriptionsBuilderForProducerMicroservice(consumer_tenant, producer_microservice);
        };

        Because of = () => exception = Catch.Exception(() => builder.BuildAndSubscribe(event_horizons.Object, CancellationToken.None));

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<SubscriptionDefinitionIncomplete>();
    }
}
