// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Tenancy;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Defines a system for subscribing to event horizons.
    /// </summary>
    public interface ISubscriptions
    {
        /// <summary>
        /// Notifies the runtime to subscribe to an event horizon.
        /// </summary>
        /// <param name="consumerTenant">The consumer <see cref="TenantId"/>.</param>
        /// <param name="subscription">The <see cref="Subscription"/> that describes the subscription.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="SubscriptionResponse"/>.</returns>
        Task<SubscriptionResponse> Subscribe(TenantId consumerTenant, Subscription subscription, CancellationToken cancellationToken);
    }
}