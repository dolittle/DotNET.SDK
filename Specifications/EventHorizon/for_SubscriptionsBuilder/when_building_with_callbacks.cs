// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionsBuilder
{
    public class when_building_with_callbacks : given.event_horizons_with_observable_responses
    {
        static Mock<SubscriptionSucceeded> succeeded_handler;
        static Mock<SubscriptionFailed> failed_handler;
        static Mock<SubscriptionCompleted> completed_handler;

        static SubscriptionsBuilder builder;

        Establish context = () =>
        {
            succeeded_handler = new Mock<SubscriptionSucceeded>();
            failed_handler = new Mock<SubscriptionFailed>();
            completed_handler = new Mock<SubscriptionCompleted>();

            builder = new SubscriptionsBuilder();

            builder.OnSuccess(succeeded_handler.Object);
            builder.OnFailure(failed_handler.Object);
            builder.OnCompleted(completed_handler.Object);
        };

        Because of = () => builder.BuildAndSubscribe(event_horizons, CancellationToken.None);

        It should_call_the_succeded_handler_with_the_successful_response = () => succeeded_handler.Verify(_ => _.Invoke(subscription, success_response.Consent), Times.Once());
        It should_call_the_failed_handler_with_the_failed_response = () => failed_handler.Verify(_ => _.Invoke(subscription, failed_response.Failure));
        It should_call_the_completed_handler_with_the_successful_response = () => completed_handler.Verify(_ => _.Invoke(success_response));
        It should_call_the_completed_handler_with_the_failed_response = () => completed_handler.Verify(_ => _.Invoke(failed_response));
    }
}
