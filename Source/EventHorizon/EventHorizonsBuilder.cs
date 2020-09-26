// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents a builder for building event horizons.
    /// </summary>
    public class EventHorizonsBuilder
    {
        readonly IList<TenantSubscriptionsBuilder> _builders = new List<TenantSubscriptionsBuilder>();

        /// <summary>
        /// Sets the event horizon subscriptions for the consumer tenant through the <see cref="TenantSubscriptionsBuilder"/>.
        /// </summary>
        /// <param name="consumerTenantId">The <see cref="TenantId"/> to subscribe to events for.</param>
        /// <param name="callback">The builder callback.</param>
        /// <returns>Continuation of the builder.</returns>
        public EventHorizonsBuilder ForTenant(TenantId consumerTenantId, Action<TenantSubscriptionsBuilder> callback)
        {
            var builder = new TenantSubscriptionsBuilder(consumerTenantId);
            callback(builder);
            _builders.Add(builder);
            return this;
        }

        /// <summary>
        /// Builds and registers the event horizon subscriptions.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="IEventHorizons"/> to use for subscribing.</param>
        public void BuildAndSubscribe(IEventHorizons eventHorizons)
        {
            foreach (var builder in _builders)
            {
                builder.BuildAndSubscribe(eventHorizons);
            }
        }
    }
}