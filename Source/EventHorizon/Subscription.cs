// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;
using Dolittle.Tenancy;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Represents the configuration of an event horizon subscription.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Gets or sets the <see cref="ScopeId" />.
        /// </summary>
        public ScopeId Scope { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Microservice" /> to receive events from.
        /// </summary>
        public ApplicationModel.Microservice Microservice { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId" /> tenant to receive events from.
        /// </summary>
        public TenantId Tenant { get; set; }

        /// <summary>
        /// Gets or sets the public <see cref="StreamId" /> to subscribe to.
        /// </summary>
        public StreamId Stream { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartitionId" /> in the public stream.
        /// </summary>
        public PartitionId Partition { get; set; }
    }
}