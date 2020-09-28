// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.SDK.EventHorizon.Internal;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Machine.Specifications;
using SubscriptionRequest = Dolittle.Runtime.EventHorizon.Contracts.Subscription;

namespace Dolittle.SDK.EventHorizon.for_EventHorizons.when_subscribing
{
    public class and_subscription_succeeds : given.an_event_horizons_and_a_subscription
    {
        static Uuid consent_id;
        static SubscriptionResponse response_to_return;

        static SubscriptionRequest request;
        static SubscribeResponse response;

        Establish context = () =>
        {
            consent_id = Guid.Parse("c67e078e-83a4-4850-a931-7c04ab41e42a").ToProtobuf();

            response_to_return = new SubscriptionResponse
            {
                ConsentId = consent_id,
                Failure = null,
            };

            caller
                .Setup(_ => _.Call(Moq.It.IsAny<SubscriptionsSubscribeMethod>(), Moq.It.IsAny<SubscriptionRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Callback<ICanCallAUnaryMethod<SubscriptionRequest, SubscriptionResponse>, SubscriptionRequest, CancellationToken>((m, r, t) => request = r)
                .Returns(Task.FromResult(response_to_return));
        };

        Because of = () => response = event_horizons.Subscribe(subscription).GetAwaiter().GetResult();

        It should_send_a_request_with_the_correct_execution_context = () => request.CallContext.ExecutionContext.ShouldEqual(execution_context.ForTenant(subscription.ConsumerTenant).ToProtobuf());
        It should_send_a_request_with_the_correct_microservice_id = () => request.MicroserviceId.ShouldEqual(subscription.ProducerMicroservice.ToProtobuf());
        It should_send_a_request_with_the_correct_tenant_id = () => request.TenantId.ShouldEqual(subscription.ProducerTenant.ToProtobuf());
        It should_send_a_request_with_the_correct_stream_id = () => request.StreamId.ShouldEqual(subscription.ProducerStream.ToProtobuf());
        It should_send_a_request_with_the_correct_partition_id = () => request.PartitionId.ShouldEqual(subscription.ProducerPartition.ToProtobuf());
        It should_send_a_request_with_the_correct_scope_id = () => request.ScopeId.ShouldEqual(subscription.ConsumerScope.ToProtobuf());
        It should_return_a_response_that_is_not_failed = () => response.Failed.ShouldBeFalse();
        It should_return_the_correct_consent_id = () => response.Consent.ShouldEqual(consent_id.To<ConsentId>());
        It should_not_set_the_failure = () => response.Failure.ShouldBeNull();
    }
}
