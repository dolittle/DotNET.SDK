// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice, producer tenant, producer stream, producer partition and consumer scope already defined.
    /// </summary>
    public class TenantSubscriptionToScopeBuilder
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        readonly TenantId _producerTenantId;
        readonly StreamId _producerStreamId;
        readonly PartitionId _producerPartitionId;
        readonly ScopeId _consumerScopeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSubscriptionToScopeBuilder"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerStreamId">The producer <see cref="StreamId"/> of the subscription.</param>
        /// <param name="producerPartitionId">The producer <see cref="PartitionId"/> of the subscription.</param>
        /// <param name="consumerScopeId">The consumer <see cref="ScopeId"/> of the subscription.</param>
        public TenantSubscriptionToScopeBuilder(
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
        /// Builds the <see cref="Subscription"/>.
        /// </summary>
        /// <returns>The event handler subscription definition.</returns>
        public Subscription Build()
        {
            return new Subscription(
                _consumerTenantId,
                _producerMicroserviceId,
                _producerTenantId,
                _producerStreamId,
                _producerPartitionId,
                _consumerScopeId);
        }
    }
}
