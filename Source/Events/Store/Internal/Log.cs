// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Store.Internal;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Committing {NumberOfEvents} events for aggregate root type {AggregateRoot} and id {EventSource} with expected version {ExpectedVersion}")]
    internal static partial void CommittingAggregateEvents(ILogger logger, int numberOfEvents, AggregateRootId aggregateRoot, EventSourceId eventSource, AggregateRootVersion expectedVersion);
    
    [LoggerMessage(0, LogLevel.Debug, "Committing {NumberOfEvents} events")]
    internal static partial void CommittingEvents(ILogger logger, int numberOfEvents);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert UncommittedAggregateEvents to Protobuf.")]
    internal static partial void UncommittedAggregateEventsCouldNotBeConverted(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert UncommittedEvents to Protobuf.")]
    internal static partial void UncommittedEventsCouldNotBeConverted(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Error, "The Runtime acknowledges that the events have been committed, but the returned CommittedAggregateEvents could not be converted.")]
    internal static partial void CommittedAggregateEventsCouldNotBeConverted(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Error, "The Runtime acknowledges that the events have been committed, but the returned CommittedEvents could not be converted.")]
    internal static partial void CommittedEventsCouldNotBeConverted(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Debug, "Fetching events for aggregate root {AggregateRoot} and event source {EventSource}")]
    internal static partial void FetchingEventsForAggregate(ILogger logger, AggregateRootId aggregateRoot, EventSourceId eventSource);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert CommittedAggregateEvents to SDK.")]
    internal static partial void FetchedEventsForAggregateCouldNotBeConverted(ILogger logger, Exception ex);
}
