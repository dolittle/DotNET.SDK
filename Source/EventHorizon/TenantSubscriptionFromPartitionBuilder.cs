// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice, producer tenant, producer stream and producer partition already defined.
    /// </summary>
    public class TenantSubscriptionFromPartitionBuilder
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        readonly TenantId _producerTenantId;
        readonly StreamId _producerStreamId;
        readonly PartitionId _producerPartitionId;
        ScopeId _consumerScopeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSubscriptionFromPartitionBuilder"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerStreamId">The producer <see cref="StreamId"/> of the subscription.</param>
        /// <param name="producerPartitionId">The producer <see cref="PartitionId"/> of the subscription.</param>
        public TenantSubscriptionFromPartitionBuilder(
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
            _consumerScopeId = ScopeId.Default;
        }

        /// <summary>
        /// Sets the scope into which the events received over this subscription will be stored.
        /// </summary>
        /// <param name="consumerScopeId">The <see cref="ScopeId"/> to store the events in.</param>
        /// <returns>Continuation of the builder.</returns>
        public TenantSubscriptionFromPartitionBuilder ToScope(ScopeId consumerScopeId)
        {
            ThrowIfConsumerScopeIsAlreadyDefined();
            _consumerScopeId = consumerScopeId;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="Subscription"/>.
        /// </summary>
        /// <returns>The event handler subscription definition.</returns>
        public Subscription Build()
        {
            ThrowIfConsumerScopeIsNotDefined();
            return new Subscription(
                _consumerTenantId,
                _producerMicroserviceId,
                _producerTenantId,
                _producerStreamId,
                _producerPartitionId,
                _consumerScopeId);
        }

        void ThrowIfConsumerScopeIsAlreadyDefined()
        {
            if (_consumerScopeId != ScopeId.Default)
            {
                throw new SubscriptionBuilderMethodAlreadyCalled("ToScope()");
            }
        }

        void ThrowIfConsumerScopeIsNotDefined()
        {
            if (_consumerScopeId == ScopeId.Default)
            {
                throw new SubscriptionDefinitionIncomplete("Scope", "Call ToScope() with a non-default scope");
            }
        }
    }
}
