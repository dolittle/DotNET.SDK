// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building event horizons.
    /// </summary>
    public class SubscriptionsBuilder
    {
        readonly SubscriptionCallbacks _callbacks = new SubscriptionCallbacks();
        readonly IList<SubscriptionsBuilderForConsumerTenant> _builders = new List<SubscriptionsBuilderForConsumerTenant>();

        /// <summary>
        /// Sets the event horizon subscriptions for the consumer tenant through the <see cref="SubscriptionsBuilderForConsumerTenant"/>.
        /// </summary>
        /// <param name="consumerTenantId">The <see cref="TenantId"/> to subscribe to events for.</param>
        /// <param name="callback">The builder callback.</param>
        /// <returns>Continuation of the builder.</returns>
        public SubscriptionsBuilder ForTenant(TenantId consumerTenantId, Action<SubscriptionsBuilderForConsumerTenant> callback)
        {
            var builder = new SubscriptionsBuilderForConsumerTenant(consumerTenantId);
            callback(builder);
            _builders.Add(builder);
            return this;
        }

        /// <summary>
        /// Registers a success callback to be called when subscriptions succeed.
        /// </summary>
        /// <param name="callback">The <see cref="SubscriptionSucceeded"/> to call.</param>
        /// <returns>Continuation of the builder.</returns>
        public SubscriptionsBuilder OnSuccess(SubscriptionSucceeded callback)
        {
            _callbacks.OnSuccess += callback;
            return this;
        }

        /// <summary>
        /// Registers a success callback to be called when subscriptions fail.
        /// </summary>
        /// <param name="callback">The <see cref="SubscriptionFailed"/> to call.</param>
        /// <returns>Continuation of the builder.</returns>
        public SubscriptionsBuilder OnFailure(SubscriptionFailed callback)
        {
            _callbacks.OnFailure += callback;
            return this;
        }

        /// <summary>
        /// Registers a success callback to be called when subscriptions complete.
        /// </summary>
        /// <param name="callback">The <see cref="SubscriptionCompleted"/> to call.</param>
        /// <returns>Continuation of the builder.</returns>
        public SubscriptionsBuilder OnCompleted(SubscriptionCompleted callback)
        {
            _callbacks.OnCompleted += callback;
            return this;
        }

        /// <summary>
        /// Builds and registers the event horizon subscriptions.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="IEventHorizons"/> to use for subscribing.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        public void BuildAndSubscribe(IEventHorizons eventHorizons, CancellationToken cancellationToken)
        {
            eventHorizons.Responses.Subscribe(_callbacks, cancellationToken);
            foreach (var builder in _builders)
            {
                builder.BuildAndSubscribe(eventHorizons, cancellationToken);
            }
        }
    }
}