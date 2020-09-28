// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice, producer tenant and producer stream already defined.
    /// </summary>
    public class SubscriptionBuilderForProducerStream
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        readonly TenantId _producerTenantId;
        readonly StreamId _producerStreamId;
        SubscriptionBuilderForProducerPartition _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBuilderForProducerStream"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerStreamId">The producer <see cref="StreamId"/> of the subscription.</param>
        public SubscriptionBuilderForProducerStream(
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
        /// <returns>A <see cref="SubscriptionBuilderForProducerPartition"/> to continue building.</returns>
        public SubscriptionBuilderForProducerPartition FromPartition(PartitionId producerPartitionId)
        {
            ThrowIfProducerPartitionIsAlreadyDefined();
            _builder = new SubscriptionBuilderForProducerPartition(
                _consumerTenantId,
                _producerMicroserviceId,
                _producerTenantId,
                _producerStreamId,
                producerPartitionId);
            return _builder;
        }

        /// <summary>
        /// Builds and registers the event horizon subscriptions.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="IEventHorizons"/> to use for subscribing.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        public void BuildAndSubscribe(IEventHorizons eventHorizons, CancellationToken cancellationToken)
        {
            ThrowIfProducerPartitionIsNotDefined();
            _builder.BuildAndSubscribe(eventHorizons, cancellationToken);
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
