// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.EventHorizon;
/// <summary>
/// Represents a subscription definition that can be used to set up an event horizon subscription through the runtime.
/// </summary>
/// <param name="ConsumerTenant">The consumer <see cref="TenantId"/> of the subscription.</param>
/// <param name="ProducerMicroservice">The producer <see cref="MicroserviceId"/> of the subscription.</param>
/// <param name="ProducerTenant">The producer <see cref="TenantId"/> of the subscription.</param>
/// <param name="ProducerStream">The producer <see cref="StreamId"/> of the subscription.</param>
/// <param name="ProducerPartition">The producer <see cref="PartitionId"/> of the subscription.</param>
/// <param name="ConsumerScope">The consumer <see cref="ScopeId"/> of the subscription.</param>
public record Subscription(
    TenantId ConsumerTenant,
    MicroserviceId ProducerMicroservice,
    TenantId ProducerTenant,
    StreamId ProducerStream,
    PartitionId ProducerPartition,
    ScopeId ConsumerScope);
