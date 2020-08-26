// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Booting;
using Dolittle.Logging;
using Dolittle.Resilience;
using Dolittle.Tenancy;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Represents a <see cref="ICanPerformBootProcedure"/> that subscribes to event horizons using the Runtime.
    /// </summary>
    public class SubscriptionBootProcedure : ICanPerformBootProcedure
    {
        readonly EventHorizonsConfiguration _configuration;
        readonly ISubscriptions _subscriptions;
        readonly IAsyncPolicyFor<SubscriptionBootProcedure> _policy;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBootProcedure"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="EventHorizonsConfiguration"/> that contains all <see cref="Subscription"/>.</param>
        /// <param name="subscriptions">The <see cref="ISubscriptions"/> to use to subscribe.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}"/> that defines reconnect policies for the event horizon subscriptions.</param>
        /// <param name="logger">The <see cref="ILogger"/> used for logging.</param>
        public SubscriptionBootProcedure(
            EventHorizonsConfiguration configuration,
            ISubscriptions subscriptions,
            IAsyncPolicyFor<SubscriptionBootProcedure> policy,
            ILogger logger)
        {
            _configuration = configuration;
            _subscriptions = subscriptions;
            _policy = policy;
            _logger = logger;
        }

        /// <inheritdoc/>
        public bool CanPerform() => Microservice.Configuration.BootProcedure.HasPerformed;

        /// <inheritdoc/>
        public void Perform()
        {
            foreach ((TenantId consumer, IEnumerable<Subscription> subscriptions) in _configuration)
            {
                foreach (var subscription in subscriptions)
                {
                    Task.Run(() => Subscribe(consumer, subscription));
                }
            }
        }

        Task Subscribe(TenantId consumer, Subscription subscription)
            => _policy.Execute(
                async (cancellationToken) =>
                {
                    _logger.Trace(
                            "Attempting to subscribe to events from {Partition} in {Stream} of {ProducerTenant} in {Microservice} for {ConsumerTenant} into {Scope}",
                            subscription.Partition,
                            subscription.Stream,
                            subscription.Tenant,
                            subscription.Microservice,
                            consumer,
                            subscription.Scope);
                    var response = await _subscriptions.Subscribe(consumer, subscription, cancellationToken).ConfigureAwait(false);
                    if (!response.Success)
                    {
                        throw new FailedToSubscribeToEventHorizon(
                            response.Failure.Reason,
                            consumer,
                            subscription.Microservice,
                            subscription.Tenant,
                            subscription.Stream,
                            subscription.Partition);
                    }
                    else
                    {
                        _logger.Debug(
                            "Successfully subscribed to events from {Partition} in {Stream} of {ProducerTenant} in {Microservice} for {ConsumerTenant} into {Scope} approved by {Consent}",
                            subscription.Partition,
                            subscription.Stream,
                            subscription.Tenant,
                            subscription.Microservice,
                            consumer,
                            subscription.Scope,
                            response.Consent);
                    }
                },
                CancellationToken.None);
    }
}