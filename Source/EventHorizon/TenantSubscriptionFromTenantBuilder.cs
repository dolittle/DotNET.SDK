// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice and producer tenant already defined.
    /// </summary>
    public class TenantSubscriptionFromTenantBuilder
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        readonly TenantId _producerTenantId;
        TenantSubscriptionFromStreamBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSubscriptionFromTenantBuilder"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
        public TenantSubscriptionFromTenantBuilder(
            TenantId consumerTenantId,
            MicroserviceId producerMicroserviceId,
            TenantId producerTenantId)
        {
            _consumerTenantId = consumerTenantId;
            _producerMicroserviceId = producerMicroserviceId;
            _producerTenantId = producerTenantId;
            _builder = null;
        }

        /// <summary>
        /// Sets the producer stream to subscribe to events from.
        /// </summary>
        /// <param name="producerStreamId">The <see cref="StreamId"/> to subscribe to events from.</param>
        /// <returns>A <see cref="TenantSubscriptionFromStreamBuilder"/> to continue building.</returns>
        public TenantSubscriptionFromStreamBuilder FromStream(StreamId producerStreamId)
        {
            ThrowIfProducerStreamIsAlreadyDefined();
            _builder = new TenantSubscriptionFromStreamBuilder(
                _consumerTenantId,
                _producerMicroserviceId,
                _producerTenantId,
                producerStreamId);
            return _builder;
        }

        /// <summary>
        /// Builds the <see cref="Subscription"/>.
        /// </summary>
        /// <returns>The event handler subscription definition.</returns>
        public Subscription Build()
        {
            ThrowIfProducerStreamIsNotDefined();
            return _builder.Build();
        }

        void ThrowIfProducerStreamIsAlreadyDefined()
        {
            if (_builder != null)
            {
                throw new SubscriptionBuilderMethodAlreadyCalled("FromStream()");
            }
        }

        void ThrowIfProducerStreamIsNotDefined()
        {
            if (_builder == null)
            {
                throw new SubscriptionDefinitionIncomplete("Stream", "Call FromStream()");
            }
        }
    }
}
