// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon;

/// <summary>
/// Represents a builder for building event horizon subscriptions for a consumer tenant.
/// </summary>
public class SubscriptionsBuilderForConsumerTenant
{
    readonly SubscriptionCallbacks _callbacks = new();
    readonly IList<SubscriptionBuilderForProducerMicroservice> _builders = [];
    readonly TenantId _consumerTenantId;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionsBuilderForConsumerTenant"/> class.
    /// </summary>
    /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
    public SubscriptionsBuilderForConsumerTenant(
        TenantId consumerTenantId)
    {
        _consumerTenantId = consumerTenantId;
    }

    /// <summary>
    /// Sets the producer microservice to subscribe to events from.
    /// </summary>
    /// <param name="microserviceId">The <see cref="MicroserviceId"/> to subscribe to events from.</param>
    /// <returns>A <see cref="SubscriptionBuilderForProducerMicroservice"/> to continue building.</returns>
    public SubscriptionBuilderForProducerMicroservice FromProducerMicroservice(MicroserviceId microserviceId)
    {
        var builder = new SubscriptionBuilderForProducerMicroservice(_consumerTenantId, microserviceId);
        _builders.Add(builder);
        return builder;
    }

    /// <summary>
    /// Registers a success callback to be called when subscriptions for this tenant succeed.
    /// </summary>
    /// <param name="callback">The <see cref="SubscriptionSucceeded"/> to call.</param>
    /// <returns>Continuation of the builder.</returns>
    public SubscriptionsBuilderForConsumerTenant OnSuccess(SubscriptionSucceeded callback)
    {
        _callbacks.OnSuccess += callback;
        return this;
    }

    /// <summary>
    /// Registers a success callback to be called when subscriptions for this tenant fail.
    /// </summary>
    /// <param name="callback">The <see cref="SubscriptionFailed"/> to call.</param>
    /// <returns>Continuation of the builder.</returns>
    public SubscriptionsBuilderForConsumerTenant OnFailure(SubscriptionFailed callback)
    {
        _callbacks.OnFailure += callback;
        return this;
    }

    /// <summary>
    /// Registers a success callback to be called when subscriptions for this tenant complete.
    /// </summary>
    /// <param name="callback">The <see cref="SubscriptionCompleted"/> to call.</param>
    /// <returns>Continuation of the builder.</returns>
    public SubscriptionsBuilderForConsumerTenant OnCompleted(SubscriptionCompleted callback)
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
        eventHorizons.Responses.Where(_ => _.Subscription.ConsumerTenant == _consumerTenantId).Subscribe(_callbacks, cancellationToken);
        foreach (var builder in _builders)
        {
            builder.BuildAndSubscribe(eventHorizons, cancellationToken);
        }
    }
}
