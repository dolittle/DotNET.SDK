// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Aggregates"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug,"Performing operation on {AggregateRoot} with aggregate root id {AggregateRootId} applying events to event source {EventSource}")]
    internal static partial void PerformingOn(this ILogger logger, Type aggregateRoot, AggregateRootId aggregateRootId, EventSourceId eventSource);
    
    [LoggerMessage(0, LogLevel.Debug, "Rehydrating {AggregateRoot} with aggregate root id {AggregateRootId} with event source id {EventSource}")]
    internal static partial void RehydratingAggregateRoot(this ILogger logger, Type aggregateRoot, AggregateRootId aggregateRootId, EventSourceId eventSource);

    [LoggerMessage(0, LogLevel.Trace, "Re-applying {NumberOfEvents} events")]
    internal static partial void ReApplying(this ILogger logger, int numberOfEvents);
    
    [LoggerMessage(0, LogLevel.Trace, "No events to re-apply")]
    internal static partial void NoEventsToReApply(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Debug, "{AggregateRoot} with aggregate root id {AggregateRootId} is committing {NumberOfEvents} events to event source {EventSource}")]
    internal static partial void CommittingEvents(this ILogger logger, Type aggregateRoot, AggregateRootId aggregateRootId, int numberOfEvents, EventSourceId eventSource);
}
