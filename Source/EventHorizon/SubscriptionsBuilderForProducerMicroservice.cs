// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant and producer microservice already defined.
    /// </summary>
    public class SubscriptionsBuilderForProducerMicroservice
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        SubscriptionBuilderForProducerTenant _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsBuilderForProducerMicroservice"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        public SubscriptionsBuilderForProducerMicroservice(
            TenantId consumerTenantId,
            MicroserviceId producerMicroserviceId)
        {
            _consumerTenantId = consumerTenantId;
            _producerMicroserviceId = producerMicroserviceId;
            _builder = null;
        }

        /// <summary>
        /// Sets the producer tenant to subscribe to events from.
        /// </summary>
        /// <param name="producerTenantId">The <see cref="TenantId"/> to subscribe to events from.</param>
        /// <returns>A <see cref="SubscriptionBuilderForProducerTenant"/> to continue building.</returns>
        public SubscriptionBuilderForProducerTenant FromTenant(TenantId producerTenantId)
        {
            ThrowIfProducerTenantIsAlreadyDefined();
            _builder = new SubscriptionBuilderForProducerTenant(
                _consumerTenantId,
                _producerMicroserviceId,
                producerTenantId);
            return _builder;
        }

        /// <summary>
        /// Builds and registers the event horizon subscriptions.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="IEventHorizons"/> to use for subscribing.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        public void BuildAndSubscribe(IEventHorizons eventHorizons, CancellationToken cancellationToken)
        {
            ThrowIfProducerTenantIsNotDefined();
            _builder.BuildAndSubscribe(eventHorizons, cancellationToken);
        }

        void ThrowIfProducerTenantIsAlreadyDefined()
        {
            if (_builder != null)
            {
                throw new SubscriptionBuilderMethodAlreadyCalled("FromTenant()");
            }
        }

        void ThrowIfProducerTenantIsNotDefined()
        {
            if (_builder == null)
            {
                throw new SubscriptionDefinitionIncomplete("Producer Tenant", "Call FromTenant()");
            }
        }
    }
}
