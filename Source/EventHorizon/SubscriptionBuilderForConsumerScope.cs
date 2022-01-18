// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon;

/// <summary>
/// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice, producer tenant, producer stream, producer partition and consumer scope already defined.
/// </summary>
public class SubscriptionBuilderForConsumerScope
{
    readonly SubscriptionCallbacks _callbacks = new();
    readonly TenantId _consumerTenantId;
    readonly MicroserviceId _producerMicroserviceId;
    readonly TenantId _producerTenantId;
    readonly StreamId _producerStreamId;
    readonly PartitionId _producerPartitionId;
    readonly ScopeId _consumerScopeId;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionBuilderForConsumerScope"/> class.
    /// </summary>
    /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
    /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
    /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
    /// <param name="producerStreamId">The producer <see cref="StreamId"/> of the subscription.</param>
    /// <param name="producerPartitionId">The producer <see cref="PartitionId"/> of the subscription.</param>
    /// <param name="consumerScopeId">The consumer <see cref="ScopeId"/> of the subscription.</param>
    public SubscriptionBuilderForConsumerScope(
        TenantId consumerTenantId,
        MicroserviceId producerMicroserviceId,
        TenantId producerTenantId,
        StreamId producerStreamId,
        PartitionId producerPartitionId,
        ScopeId consumerScopeId)
    {
        _consumerTenantId = consumerTenantId;
        _producerMicroserviceId = producerMicroserviceId;
        _producerTenantId = producerTenantId;
        _producerStreamId = producerStreamId;
        _producerPartitionId = producerPartitionId;
        _consumerScopeId = consumerScopeId;
    }

    /// <summary>
    /// Registers a success callback to be called if this subscription succeeds.
    /// </summary>
    /// <param name="callback">The <see cref="SubscriptionSucceeded"/> to call.</param>
    /// <returns>Continuation of the builder.</returns>
    public SubscriptionBuilderForConsumerScope OnSuccess(SubscriptionSucceeded callback)
    {
        _callbacks.OnSuccess += callback;
        return this;
    }

    /// <summary>
    /// Registers a success callback to be called if this subscription fails.
    /// </summary>
    /// <param name="callback">The <see cref="SubscriptionFailed"/> to call.</param>
    /// <returns>Continuation of the builder.</returns>
    public SubscriptionBuilderForConsumerScope OnFailure(SubscriptionFailed callback)
    {
        _callbacks.OnFailure += callback;
        return this;
    }

    /// <summary>
    /// Registers a success callback to be called when this subscription completes.
    /// </summary>
    /// <param name="callback">The <see cref="SubscriptionCompleted"/> to call.</param>
    /// <returns>Continuation of the builder.</returns>
    public SubscriptionBuilderForConsumerScope OnCompleted(SubscriptionCompleted callback)
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
        var subscription = new Subscription(
            _consumerTenantId,
            _producerMicroserviceId,
            _producerTenantId,
            _producerStreamId,
            _producerPartitionId,
            _consumerScopeId);

        eventHorizons.Responses.Where(_ => _.Subscription == subscription).Subscribe(_callbacks, cancellationToken);

        eventHorizons.Subscribe(subscription);
    }
}