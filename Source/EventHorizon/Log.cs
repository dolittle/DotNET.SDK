// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.EventHorizon;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.EventHorizon"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Subscribing to events from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope}")]
    internal static partial void SubscribingTo(
        this ILogger logger,
        MicroserviceId producerMicroservice,
        TenantId producerTenant,
        StreamId producerStream,
        PartitionId producerPartition,
        TenantId consumerTenant,
        ScopeId consumerScope);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to subscribe to events from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope} because {Reason}")]
    internal static partial void FailedToSubscribeTo(
        this ILogger logger,
        MicroserviceId producerMicroservice,
        TenantId producerTenant,
        StreamId producerStream,
        PartitionId producerPartition,
        TenantId consumerTenant,
        ScopeId consumerScope,
        FailureReason reason);
    
    [LoggerMessage(0, LogLevel.Debug, "Successfully subscribed to events from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope} with {Consent}")]
    internal static partial void SuccessfullySubscribedTo(
        this ILogger logger,
        MicroserviceId producerMicroservice,
        TenantId producerTenant,
        StreamId producerStream,
        PartitionId producerPartition,
        TenantId consumerTenant,
        ScopeId consumerScope,
        ConsentId consent);

    [LoggerMessage(0, LogLevel.Warning, "An exception was thrown while registering an event horizon subscription.")]
    internal static partial void ErrorWhileRegisteringSubscription(this ILogger logger, Exception exception);
}
