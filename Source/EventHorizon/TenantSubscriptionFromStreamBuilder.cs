// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice, producer tenant and producer stream already defined.
    /// </summary>
    public class TenantSubscriptionFromStreamBuilder
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        readonly TenantId _producerTenantId;
        readonly StreamId _producerStreamId;
        TenantSubscriptionFromPartitionBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSubscriptionFromStreamBuilder"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerStreamId">The producer <see cref="StreamId"/> of the subscription.</param>
        public TenantSubscriptionFromStreamBuilder(
            TenantId consumerTenantId,
            MicroserviceId producerMicroserviceId,
            TenantId producerTenantId,
            StreamId producerStreamId)
        {
            _consumerTenantId = consumerTenantId;
            _producerMicroserviceId = producerMicroserviceId;
            _producerTenantId = producerTenantId;
            _producerStreamId = producerStreamId;
            _builder = null;
        }

        /// <summary>
        /// Sets the partition of the producer stream to subscribe to events from.
        /// </summary>
        /// <param name="producerPartitionId">The <see cref="PartitionId"/> to subscribe to events from.</param>
        /// <returns>A <see cref="TenantSubscriptionFromPartitionBuilder"/> to continue building.</returns>
        public TenantSubscriptionFromPartitionBuilder FromPartition(PartitionId producerPartitionId)
        {
            ThrowIfProducerPartitionIsAlreadyDefined();
            _builder = new TenantSubscriptionFromPartitionBuilder(
                _consumerTenantId,
                _producerMicroserviceId,
                _producerTenantId,
                _producerStreamId,
                producerPartitionId);
            return _builder;
        }

        /// <summary>
        /// Builds the <see cref="Subscription"/>.
        /// </summary>
        /// <returns>The event handler subscription definition.</returns>
        public Subscription Build()
        {
            ThrowIfProducerPartitionIsNotDefined();
            return _builder.Build();
        }

        void ThrowIfProducerPartitionIsAlreadyDefined()
        {
            if (_builder != null)
            {
                throw new SubscriptionBuilderMethodAlreadyCalled("FromPartition()");
            }
        }

        void ThrowIfProducerPartitionIsNotDefined()
        {
            if (_builder == null)
            {
                throw new SubscriptionDefinitionIncomplete("Partition", "Call FromPartition()");
            }
        }
    }
}
