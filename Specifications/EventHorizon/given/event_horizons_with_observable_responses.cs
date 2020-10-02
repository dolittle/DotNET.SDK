// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.EventHorizon.given
{
    public class event_horizons_with_observable_responses
    {
        protected static TenantId tenant_one;
        protected static TenantId tenant_two;

        protected static Subscription subscription_tenant_one;
        protected static Subscription subscription_one_tenant_two;
        protected static Subscription subscription_two_tenant_two;

        protected static ConsentId consent_id;
        protected static Failure failure;

        protected static SubscribeResponse response_subscription_tenant_one_success;
        protected static SubscribeResponse response_subscription_tenant_one_failure;
        protected static SubscribeResponse response_subscription_one_tenant_two_success;
        protected static SubscribeResponse response_subscription_one_tenant_two_failure;
        protected static SubscribeResponse response_subscription_two_tenant_two_success;
        protected static SubscribeResponse response_subscription_two_tenant_two_failure;

        protected static IEventHorizons event_horizons;

        Establish context = () =>
        {
            tenant_one = "284c6630-956d-45d6-bae2-074163bfc4ac";
            subscription_tenant_one = new Subscription(
                tenant_one,
                "90099b66-8811-4e04-b376-458e0a30d056",
                "e5a9f65a-0fb7-4d5d-8d5a-81b445252453",
                "7d9da3fb-b977-4990-998a-10e39a4768b2",
                "7b3a196b-b25c-42a1-90e9-2ad3af2cd33f",
                "2e352cec-f365-44dc-b948-d3ef68647f9c");

            tenant_two = "4a4bd016-ec36-4378-a6c9-bfe37ff6c464";
            subscription_one_tenant_two = new Subscription(
                tenant_two,
                "90099b66-8811-4e04-b376-458e0a30d056",
                "e5a9f65a-0fb7-4d5d-8d5a-81b445252453",
                "7d9da3fb-b977-4990-998a-10e39a4768b2",
                "7b3a196b-b25c-42a1-90e9-2ad3af2cd33f",
                "2e352cec-f365-44dc-b948-d3ef68647f9c");
            subscription_two_tenant_two = new Subscription(
                tenant_two,
                "76ccf54d-2db5-4725-8495-3e9124360981",
                "e5a9f65a-0fb7-4d5d-8d5a-81b445252453",
                "7d9da3fb-b977-4990-998a-10e39a4768b2",
                "7b3a196b-b25c-42a1-90e9-2ad3af2cd33f",
                "2e352cec-f365-44dc-b948-d3ef68647f9c");

            consent_id = "a99f46e8-051a-49d8-8727-69805b6fe3c4";
            failure = new Failure(FailureId.Create("75b7337c-dedc-41e4-9fab-7d8e77c33d2c"), "Something went wrong");

            response_subscription_tenant_one_success = new SubscribeResponse(subscription_tenant_one, consent_id, null);
            response_subscription_tenant_one_failure = new SubscribeResponse(subscription_tenant_one, ConsentId.NotSet, failure);
            response_subscription_one_tenant_two_success = new SubscribeResponse(subscription_one_tenant_two, consent_id, null);
            response_subscription_one_tenant_two_failure = new SubscribeResponse(subscription_one_tenant_two, ConsentId.NotSet, failure);
            response_subscription_two_tenant_two_success = new SubscribeResponse(subscription_two_tenant_two, consent_id, null);
            response_subscription_two_tenant_two_failure = new SubscribeResponse(subscription_two_tenant_two, ConsentId.NotSet, failure);

            var observableMock = new Mock<IObservable<SubscribeResponse>>();
            observableMock
                .Setup(_ => _.Subscribe(Moq.It.IsAny<IObserver<SubscribeResponse>>()))
                .Callback<IObserver<SubscribeResponse>>(observer =>
                {
                    observer.OnNext(response_subscription_tenant_one_success);
                    observer.OnNext(response_subscription_tenant_one_failure);
                    observer.OnNext(response_subscription_one_tenant_two_success);
                    observer.OnNext(response_subscription_one_tenant_two_failure);
                    observer.OnNext(response_subscription_two_tenant_two_success);
                    observer.OnNext(response_subscription_two_tenant_two_failure);
                    observer.OnCompleted();
                });

            var eventHorizonsMock = new Mock<IEventHorizons>();
            eventHorizonsMock.SetupGet(_ => _.Responses).Returns(observableMock.Object);

            event_horizons = eventHorizonsMock.Object;
        };
    }
}