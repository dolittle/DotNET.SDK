// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionBuilderForConsumerScope;

public class when_building_with_callbacks : given.event_horizons_with_observable_responses
{
    static Mock<SubscriptionSucceeded> succeeded_handler;
    static Mock<SubscriptionFailed> failed_handler;
    static Mock<SubscriptionCompleted> completed_handler;

    static SubscriptionBuilderForConsumerScope builder;

    Establish context = () =>
    {
        succeeded_handler = new Mock<SubscriptionSucceeded>();
        failed_handler = new Mock<SubscriptionFailed>();
        completed_handler = new Mock<SubscriptionCompleted>();

        builder = new SubscriptionBuilderForConsumerScope(
            subscription_two_tenant_two.ConsumerTenant,
            subscription_two_tenant_two.ProducerMicroservice,
            subscription_two_tenant_two.ProducerTenant,
            subscription_two_tenant_two.ProducerStream,
            subscription_two_tenant_two.ProducerPartition,
            subscription_two_tenant_two.ConsumerScope);

        builder.OnSuccess(succeeded_handler.Object);
        builder.OnFailure(failed_handler.Object);
        builder.OnCompleted(completed_handler.Object);
    };

    Because of = () => builder.BuildAndSubscribe(event_horizons, CancellationToken.None);

    It should_call_the_succeded_handler_with_subscription_two_tenant_two_success = () => succeeded_handler.Verify(_ => _.Invoke(subscription_two_tenant_two, consent_id), Times.Once());
    It should_call_the_failed_handler_with_subscription_two_tenant_two_failure = () => failed_handler.Verify(_ => _.Invoke(subscription_two_tenant_two, failure), Times.Once());
    It should_call_the_completed_handler_with_subscription_two_tenant_two_success = () => completed_handler.Verify(_ => _.Invoke(response_subscription_two_tenant_two_success), Times.Once());
    It should_call_the_completed_handler_with_subscription_two_tenant_two_failure = () => completed_handler.Verify(_ => _.Invoke(response_subscription_two_tenant_two_failure), Times.Once());
}