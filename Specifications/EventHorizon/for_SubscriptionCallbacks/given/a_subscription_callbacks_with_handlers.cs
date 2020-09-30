// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionCallbacks.given
{
    public class a_subscription_callbacks_with_handlers
    {
        protected static Mock<SubscriptionSucceeded> succeeded_handler;
        protected static Mock<SubscriptionFailed> failed_handler;
        protected static Mock<SubscriptionCompleted> completed_handler;

        protected static SubscriptionCallbacks subscription_callbacks;

        Establish context = () =>
        {
            succeeded_handler = new Mock<SubscriptionSucceeded>();
            failed_handler = new Mock<SubscriptionFailed>();
            completed_handler = new Mock<SubscriptionCompleted>();

            subscription_callbacks = new SubscriptionCallbacks();

            subscription_callbacks.OnSuccess += succeeded_handler.Object;
            subscription_callbacks.OnFailure += failed_handler.Object;
            subscription_callbacks.OnCompleted += completed_handler.Object;
        };
    }
}