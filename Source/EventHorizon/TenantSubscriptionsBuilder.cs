// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building event horizon subscriptions for a consumer tenant.
    /// </summary>
    public class TenantSubscriptionsBuilder
    {
        readonly SubscriptionCallbacks _callbacks = new SubscriptionCallbacks();
        readonly IList<TenantSubscriptionFromMicroserviceBuilder> _builders = new List<TenantSubscriptionFromMicroserviceBuilder>();
        readonly TenantId _consumerTenantId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSubscriptionsBuilder"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        public TenantSubscriptionsBuilder(
            TenantId consumerTenantId)
        {
            _consumerTenantId = consumerTenantId;
        }

        /// <summary>
        /// Sets the event horizon subscriptions for the consumer tenant from a producer microservice through the <see cref="TenantSubscriptionFromMicroserviceBuilder"/>.
        /// </summary>
        /// <param name="producerMicroserviceId">The <see cref="MicroserviceId"/> to subscribe to events from.</param>
        /// <param name="callback">The builder callback.</param>
        /// <returns>Continuation of the builder.</returns>
        public TenantSubscriptionsBuilder FromMicroservice(MicroserviceId producerMicroserviceId, Action<TenantSubscriptionFromMicroserviceBuilder> callback)
        {
            var builder = new TenantSubscriptionFromMicroserviceBuilder(_consumerTenantId, producerMicroserviceId);
            callback(builder);
            _builders.Add(builder);
            return this;
        }

        /// <summary>
        /// Registers a success callback to be called when subscriptions for this tenant succeed.
        /// </summary>
        /// <param name="callback">The <see cref="SubscriptionSucceeded"/> to call.</param>
        /// <returns>Continuation of the builder.</returns>
        public TenantSubscriptionsBuilder OnSuccess(SubscriptionSucceeded callback)
        {
            _callbacks.OnSuccess += callback;
            return this;
        }

        /// <summary>
        /// Registers a success callback to be called when subscriptions for this tenant fail.
        /// </summary>
        /// <param name="callback">The <see cref="SubscriptionFailed"/> to call.</param>
        /// <returns>Continuation of the builder.</returns>
        public TenantSubscriptionsBuilder OnFailure(SubscriptionFailed callback)
        {
            _callbacks.OnFailure += callback;
            return this;
        }

        /// <summary>
        /// Registers a success callback to be called when subscriptions for this tenant complete.
        /// </summary>
        /// <param name="callback">The <see cref="SubscriptionCompleted"/> to call.</param>
        /// <returns>Continuation of the builder.</returns>
        public TenantSubscriptionsBuilder OnCompleted(SubscriptionCompleted callback)
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
            eventHorizons.Responses.Where(_ => _.Subscribtion.ConsumerTenant == _consumerTenantId).Subscribe(_callbacks, cancellationToken);
            foreach (var builder in _builders)
            {
                builder.BuildAndSubscribe(eventHorizons, cancellationToken);
            }
        }
    }
}
