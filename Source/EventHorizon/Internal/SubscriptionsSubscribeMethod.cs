// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;
using static Dolittle.Runtime.EventHorizon.Contracts.Subscriptions;
using SubscriptionRequest = Dolittle.Runtime.EventHorizon.Contracts.Subscription;

namespace Dolittle.SDK.EventHorizon.Internal
{
    /// <summary>
    /// Represents a wrapper for gRPC Subscriptions.Subscribe.
    /// </summary>
    public class SubscriptionsSubscribeMethod : ICanCallAUnaryMethod<SubscriptionsClient, SubscriptionRequest, SubscriptionResponse>
    {
        /// <inheritdoc/>
        public AsyncUnaryCall<SubscriptionResponse> Call(SubscriptionRequest message, Channel channel, CallOptions callOptions)
        {
            var client = new SubscriptionsClient(channel);
            return client.SubscribeAsync(message, callOptions);
        }
    }
}