// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice, producer tenant, producer stream and producer partition already defined.
    /// </summary>
    public class SubscriptionBuilderForProducerPartition
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        readonly TenantId _producerTenantId;
        readonly StreamId _producerStreamId;
        readonly PartitionId _producerPartitionId;
        SubscriptionBuilderForConsumerScope _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBuilderForProducerPartition"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerStreamId">The producer <see cref="StreamId"/> of the subscription.</param>
        /// <param name="producerPartitionId">The producer <see cref="PartitionId"/> of the subscription.</param>
        public SubscriptionBuilderForProducerPartition(
            TenantId consumerTenantId,
            MicroserviceId producerMicroserviceId,
            TenantId producerTenantId,
            StreamId producerStreamId,
            PartitionId producerPartitionId)
        {
            _consumerTenantId = consumerTenantId;
            _producerMicroserviceId = producerMicroserviceId;
            _producerTenantId = producerTenantId;
            _producerStreamId = producerStreamId;
            _producerPartitionId = producerPartitionId;
            _builder = null;
        }

        /// <summary>
        /// Sets the scope into which the events received over this subscription will be stored.
        /// </summary>
        /// <param name="consumerScopeId">The <see cref="ScopeId"/> to store the events in.</param>
        /// <returns>A <see cref="SubscriptionBuilderForConsumerScope"/> to continue building.</returns>
        public SubscriptionBuilderForConsumerScope ToScope(ScopeId consumerScopeId)
        {
            ThrowIfConsumerScopeIsAlreadyDefined();
            _builder = new SubscriptionBuilderForConsumerScope(
                _consumerTenantId,
                _producerMicroserviceId,
                _producerTenantId,
                _producerStreamId,
                _producerPartitionId,
                consumerScopeId);
            return _builder;
        }

        /// <summary>
        /// Builds and registers the event horizon subscriptions.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="IEventHorizons"/> to use for subscribing.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        public void BuildAndSubscribe(IEventHorizons eventHorizons, CancellationToken cancellationToken)
        {
            ThrowIfConsumerScopeIsNotDefined();
            _builder.BuildAndSubscribe(eventHorizons, cancellationToken);
        }

        void ThrowIfConsumerScopeIsAlreadyDefined()
        {
            if (_builder != null)
            {
                throw new SubscriptionBuilderMethodAlreadyCalled("ToScope()");
            }
        }

        void ThrowIfConsumerScopeIsNotDefined()
        {
            if (_builder == null)
            {
                throw new SubscriptionDefinitionIncomplete("Scope", "Call ToScope() with a non-default scope");
            }
        }
    }
}
