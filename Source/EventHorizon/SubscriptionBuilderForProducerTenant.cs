// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building an event horizon subscription with consumer tenant, producer microservice and producer tenant already defined.
    /// </summary>
    public class SubscriptionBuilderForProducerTenant
    {
        readonly TenantId _consumerTenantId;
        readonly MicroserviceId _producerMicroserviceId;
        readonly TenantId _producerTenantId;
        SubscriptionBuilderForProducerStream _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBuilderForProducerTenant"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId"/> of the subscription.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="MicroserviceId"/> of the subscription.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId"/> of the subscription.</param>
        public SubscriptionBuilderForProducerTenant(
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
        /// <returns>A <see cref="SubscriptionBuilderForProducerStream"/> to continue building.</returns>
        public SubscriptionBuilderForProducerStream FromStream(StreamId producerStreamId)
        {
            ThrowIfProducerStreamIsAlreadyDefined();
            _builder = new SubscriptionBuilderForProducerStream(
                _consumerTenantId,
                _producerMicroserviceId,
                _producerTenantId,
                producerStreamId);
            return _builder;
        }

        /// <summary>
        /// Builds and registers the event horizon subscriptions.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="IEventHorizons"/> to use for subscribing.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        public void BuildAndSubscribe(IEventHorizons eventHorizons, CancellationToken cancellationToken)
        {
            ThrowIfProducerStreamIsNotDefined();
            _builder.BuildAndSubscribe(eventHorizons, cancellationToken);
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
