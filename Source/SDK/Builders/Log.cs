// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Builders;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Builders"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Retry attempt {RetryCount} processing subscription to events in {Timeout}ms from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope}")]
    internal static partial void RetryEventHorizonSubscription(
        this ILogger logger,
        int retryCount,
        TimeSpan timeout,
        MicroserviceId producerMicroservice,
        TenantId producerTenant,
        StreamId producerStream,
        PartitionId producerPartition,
        TenantId consumerTenant,
        ScopeId consumerScope);
}
