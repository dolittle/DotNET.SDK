// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionCallbacks;

public class when_calling_on_next_with_a_successful_subscription : given.a_subscription_callbacks_with_handlers
{
    static Subscription subscription;
    static ConsentId consent_id;
    static SubscribeResponse response;

    Establish context = () =>
    {
        subscription = new Subscription(
            "d32d8c55-84f7-4a98-903a-26b7aeffd4d9",
            "7e8e82f2-0090-4d3b-8c70-92afc7f3a9f1",
            "e8d50fd8-59dc-4179-b7de-a7317c4cfa0e",
            "5be03e38-7fea-4d54-913a-30374b7e8167",
            "8e23b449-4fed-494c-a545-50eee80152ea",
            "9e704dc2-23f7-459d-8698-259d8b2c6e2b");

        consent_id = "1571b160-ffb9-46b0-a310-e78965d5adcc";

        response = new SubscribeResponse(
            subscription,
            consent_id,
            null);
    };

    Because of = () => subscription_callbacks.OnNext(response);

    It should_not_invoke_on_success = () => succeeded_handler.Verify(_ => _.Invoke(subscription, consent_id), Moq.Times.Once());
    It should_not_invoke_on_failure = () => failed_handler.VerifyNoOtherCalls();
    It should_not_invoke_on_completed = () => completed_handler.Verify(_ => _.Invoke(response), Moq.Times.Once());
}