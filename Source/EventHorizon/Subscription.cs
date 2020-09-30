// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a subscription definition that can be used to set up an event horizon subscription through the runtime.
    /// </summary>
    public class Subscription : Value<Subscription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="consumerTenant">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroservice">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerStream">The producer <see cref="StreamId"/> of the subscription.</param>
        /// <param name="producerPartition">The producer <see cref="PartitionId"/> of the subscription.</param>
        /// <param name="consumerScope">The consumer <see cref="ScopeId"/> of the subscription.</param>
        public Subscription(
            TenantId consumerTenant,
            MicroserviceId producerMicroservice,
            TenantId producerTenant,
            StreamId producerStream,
            PartitionId producerPartition,
            ScopeId consumerScope)
        {
            ConsumerTenant = consumerTenant;
            ProducerMicroservice = producerMicroservice;
            ProducerTenant = producerTenant;
            ProducerStream = producerStream;
            ProducerPartition = producerPartition;
            ConsumerScope = consumerScope;
        }

        /// <summary>
        /// Gets the consumer <see cref="TenantId"/> of the subsctiption.
        /// </summary>
        public TenantId ConsumerTenant { get; }

        /// <summary>
        /// Gets the producer <see cref="MicroserviceId"/> of the subsctiption.
        /// </summary>
        public MicroserviceId ProducerMicroservice { get; }

        /// <summary>
        /// Gets the producer <see cref="TenantId"/> of the subsctiption.
        /// </summary>
        public TenantId ProducerTenant { get; }

        /// <summary>
        /// Gets the producer <see cref="StreamId"/> of the subsctiption.
        /// </summary>
        public StreamId ProducerStream { get; }

        /// <summary>
        /// Gets the producer <see cref="PartitionId"/> of the subsctiption.
        /// </summary>
        public PartitionId ProducerPartition { get; }

        /// <summary>
        /// Gets the consumer <see cref="ScopeId"/> of the subsctiption.
        /// </summary>
        public ScopeId ConsumerScope { get; }
    }
}