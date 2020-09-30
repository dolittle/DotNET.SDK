// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.SDK.EventHorizon.Internal;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using SubscriptionRequest = Dolittle.Runtime.EventHorizon.Contracts.Subscription;

namespace Dolittle.SDK.EventHorizon.for_EventHorizons
{
    public class when_observing_responses : given.an_event_horizons_and_a_subscription
    {
        static SubscribeResponse[] returned_responses;

        static List<SubscribeResponse> observed_responses;

        Establish context = () =>
        {
            caller
                .Setup(_ => _.Call(Moq.It.IsAny<SubscriptionsSubscribeMethod>(), Moq.It.IsAny<SubscriptionRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SubscriptionResponse { ConsentId = Guid.NewGuid().ToProtobuf(), Failure = null }));

            returned_responses = new[]
            {
                event_horizons.Subscribe(subscription).GetAwaiter().GetResult(),
                event_horizons.Subscribe(subscription).GetAwaiter().GetResult(),
                event_horizons.Subscribe(subscription).GetAwaiter().GetResult(),
            };

            observed_responses = new List<SubscribeResponse>();
            event_horizons.Responses.Subscribe(_ => observed_responses.Add(_));
        };

        Because of = () => event_horizons.Dispose();

        It should_observe_three_responses = () => observed_responses.Count.ShouldEqual(3);
        It should_observe_the_first_returned_response = () => observed_responses.ShouldContain(returned_responses[0]);
        It should_observe_the_second_returned_response = () => observed_responses.ShouldContain(returned_responses[1]);
        It should_observe_the_third_returned_response = () => observed_responses.ShouldContain(returned_responses[2]);
    }
}