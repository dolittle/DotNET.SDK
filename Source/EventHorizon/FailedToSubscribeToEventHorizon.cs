// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.ApplicationModel;
using Dolittle.Events;
using Dolittle.Tenancy;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when the subscription to an event horizon fails.
    /// </summary>
    public class FailedToSubscribeToEventHorizon : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToSubscribeToEventHorizon"/> class.
        /// </summary>
        /// <param name="failureReason">The failure reason.</param>
        /// <param name="subscriber">The subscriber <see cref="TenantId" />.</param>
        /// <param name="producerMicroservice">The <see cref="Microservice" /> to subscribe to.</param>
        /// <param name="producerTenant">The <see cref="TenantId" /> to subscribe to.</param>
        /// <param name="publicStream">The <see cref="StreamId" /> to subscribe to.</param>
        /// <param name="partition">The <see cref="PartitionId" /> in the stream to subscribe to.</param>
        public FailedToSubscribeToEventHorizon(string failureReason, TenantId subscriber, Microservice producerMicroservice, TenantId producerTenant, StreamId publicStream, PartitionId partition)
            : base($"Tenant {subscriber} could not subscribe to partition {partition} in public stream {publicStream} of tenant {producerTenant} in microservice {producerMicroservice}. {failureReason}")
        {
        }
    }
}