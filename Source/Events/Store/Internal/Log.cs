// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Store.Internal;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Events.Store.Internal"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Committing {NumberOfEvents} events for aggregate root type {AggregateRoot} and id {EventSource} with expected version {ExpectedVersion} to tenant {Tenant}")]
    internal static partial void CommittingAggregateEvents(this ILogger logger, int numberOfEvents, AggregateRootId aggregateRoot, EventSourceId eventSource, AggregateRootVersion expectedVersion, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Debug, "Committing {NumberOfEvents} events to tenant {Tenant}")]
    internal static partial void CommittingEvents(this ILogger logger, int numberOfEvents, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert UncommittedAggregateEvents to Protobuf.")]
    internal static partial void UncommittedAggregateEventsCouldNotBeConverted(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert UncommittedEvents to Protobuf.")]
    internal static partial void UncommittedEventsCouldNotBeConverted(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Error, "The Runtime acknowledges that the events have been committed, but the returned CommittedAggregateEvents could not be converted.")]
    internal static partial void CommittedAggregateEventsCouldNotBeConverted(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Error, "The Runtime acknowledges that the events have been committed, but the returned CommittedEvents could not be converted.")]
    internal static partial void CommittedEventsCouldNotBeConverted(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Debug, "Fetching all events from tenant {Tenant} for aggregate root {AggregateRoot} and event source {EventSource}")]
    internal static partial void FetchingAllEventsForAggregate(this ILogger logger, AggregateRootId aggregateRoot, EventSourceId eventSource, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Debug, "Fetching events from tenant {Tenant} for aggregate root {AggregateRoot} and event source {EventSource} that is of one of the following event types: [{EventTypes}")]
    internal static partial void FetchingEventsForAggregate(this ILogger logger, AggregateRootId aggregateRoot, EventSourceId eventSource, IEnumerable<EventType> eventTypes, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert CommittedAggregateEvents to SDK.")]
    internal static partial void FetchedEventsForAggregateCouldNotBeConverted(this ILogger logger, Exception ex);
}
