// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionsBuilder;

public class when_building
{
    static Mock<IEventHorizons> event_horizons;
    static SubscriptionsBuilder builder;

    static TenantId first_consumer_tenant;
    static TenantId second_consumer_tenant;

    Establish context = () =>
    {
        event_horizons = new Mock<IEventHorizons>();
        event_horizons.SetupGet(_ => _.Responses).Returns(Mock.Of<IObservable<SubscribeResponse>>());

        builder = new SubscriptionsBuilder();

        first_consumer_tenant = "c79d0dbf-c8e9-4794-8a43-bea9f589d6fa";
        builder.ForTenant(first_consumer_tenant, forTenant =>
        {
            forTenant
                .FromProducerMicroservice("08879945-401c-4224-bf8e-d45508c7486f")
                .FromProducerTenant("b194dc51-3f84-44fe-9e1c-c04634e14064")
                .FromProducerStream("9ef561dd-a1ac-4e30-baae-46674f669eae")
                .FromProducerPartition("aa440723-bc9a-46c3-8777-fadbc9f472f6")
                .ToScope("3d87171e-8b04-4cb5-92f2-966e5c7a5371");
        });

        second_consumer_tenant = "5c09714e-f20e-4f55-b0d0-bef3b99e2cdd";
        builder.ForTenant(second_consumer_tenant, forTenant =>
        {
            forTenant
                .FromProducerMicroservice("08879945-401c-4224-bf8e-d45508c7486f")
                .FromProducerTenant("b194dc51-3f84-44fe-9e1c-c04634e14064")
                .FromProducerStream("9ef561dd-a1ac-4e30-baae-46674f669eae")
                .FromProducerPartition("aa440723-bc9a-46c3-8777-fadbc9f472f6")
                .ToScope("3d87171e-8b04-4cb5-92f2-966e5c7a5371");
        });
    };

    Because of = () => builder.BuildAndSubscribe(event_horizons.Object, CancellationToken.None);

    It should_call_event_horizons_subscribe_for_the_first_tenant = () => event_horizons.Verify(_ => _.Subscribe(Moq.It.Is<Subscription>(
        _ => _.ConsumerTenant == first_consumer_tenant)));

    It should_call_event_horizons_subscribe_for_the_second_tenant = () => event_horizons.Verify(_ => _.Subscribe(Moq.It.Is<Subscription>(
        _ => _.ConsumerTenant == second_consumer_tenant)));
}