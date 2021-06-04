// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.SDK.EventHorizon.Internal;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Machine.Specifications;
using SubscriptionRequest = Dolittle.Runtime.EventHorizon.Contracts.Subscription;

namespace Dolittle.SDK.EventHorizon.for_EventHorizons.when_subscribing
{
    [Ignore("We will fix these - as the behavior has changed")]
    public class and_caller_throws_an_exception : given.an_event_horizons_and_a_subscription
    {
        static Exception exception;

        static SubscriptionRequest request;

        Establish context = () =>
        {
            exception = new Exception("Something went wrong");

            caller
                .Setup(_ => _.Call(Moq.It.IsAny<SubscriptionsSubscribeMethod>(), Moq.It.IsAny<SubscriptionRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Callback<ICanCallAUnaryMethod<SubscriptionRequest, SubscriptionResponse>, SubscriptionRequest, CancellationToken>((m, r, t) => request = r)
                .Throws(exception);
        };

        Because of = () => event_horizons.Subscribe(subscription).GetAwaiter().GetResult();

        It should_send_a_request_with_the_correct_execution_context = () => request.CallContext.ExecutionContext.ShouldEqual(execution_context.ForTenant(subscription.ConsumerTenant).ToProtobuf());
        It should_send_a_request_with_the_correct_microservice_id = () => request.MicroserviceId.ShouldEqual(subscription.ProducerMicroservice.ToProtobuf());
        It should_send_a_request_with_the_correct_tenant_id = () => request.TenantId.ShouldEqual(subscription.ProducerTenant.ToProtobuf());
        It should_send_a_request_with_the_correct_stream_id = () => request.StreamId.ShouldEqual(subscription.ProducerStream.ToProtobuf());
        It should_send_a_request_with_the_correct_partition_id = () => request.PartitionId.ShouldEqual(subscription.ProducerPartition.ToProtobuf());
        It should_send_a_request_with_the_correct_scope_id = () => request.ScopeId.ShouldEqual(subscription.ConsumerScope.ToProtobuf());
        It should_return_false_to_retry_policy = () => method_result.ShouldBeFalse();
    }
}
