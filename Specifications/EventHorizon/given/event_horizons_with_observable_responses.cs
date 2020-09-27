// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.EventHorizon.given
{
    public class event_horizons_with_observable_responses
    {
        protected static Subscription subscription;
        protected static SubscribeResponse success_response;
        protected static SubscribeResponse failed_response;
        protected static IEventHorizons event_horizons;

        Establish context = () =>
        {
            subscription = new Subscription(
                "8a90f86c-2070-47af-8811-042c5447e096",
                "90099b66-8811-4e04-b376-458e0a30d056",
                "e5a9f65a-0fb7-4d5d-8d5a-81b445252453",
                "7d9da3fb-b977-4990-998a-10e39a4768b2",
                "7b3a196b-b25c-42a1-90e9-2ad3af2cd33f",
                "2e352cec-f365-44dc-b948-d3ef68647f9c");

            success_response = new SubscribeResponse(
                subscription,
                "91ac9176-75a5-46f8-9b4e-e9862455023b",
                null);

            failed_response = new SubscribeResponse(
                subscription,
                ConsentId.NotSet,
                new Failure(
                    FailureId.Create("75b7337c-dedc-41e4-9fab-7d8e77c33d2c"),
                    "Something went wrong"));

            var observableMock = new Mock<IObservable<SubscribeResponse>>();
            observableMock
                .Setup(_ => _.Subscribe(Moq.It.IsAny<IObserver<SubscribeResponse>>()))
                .Callback<IObserver<SubscribeResponse>>(observer =>
                {
                    observer.OnNext(success_response);
                    observer.OnNext(failed_response);
                    observer.OnCompleted();
                });

            var eventHorizonsMock = new Mock<IEventHorizons>();
            eventHorizonsMock.SetupGet(_ => _.Responses).Returns(observableMock.Object);

            event_horizons = eventHorizonsMock.Object;
        };
    }
}