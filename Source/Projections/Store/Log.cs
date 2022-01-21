// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting one state from projection {Projection} of type {ProjectionType} in scope {Scope} with key {Key}")]
    internal static partial void GettingOneProjection(ILogger logger, Key key, ProjectionId projection, Type projectionType, ScopeId scope);

    [LoggerMessage(0, LogLevel.Debug, "Getting all states from projection {Projection} of type {ProjectionType} in scope {Scope}")]
    internal static partial void GettingAllProjections(ILogger logger, ProjectionId projection, Type projectionType, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Debug, "Received batch number {BatchNumber} consisting of {NumProjections} projection states")]
    internal static partial void ProcessingProjectionsInBatch(ILogger logger, int batchNumber, int numProjections);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert projection state to {ProjectionType}. State: {State}")]
    internal static partial void FailedToConvertProjection(ILogger logger, Exception exception, string state, Type projectionType);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert projection states to {ProjectionType}. States: {State}")]
    internal static partial void FailedToConvertProjection(ILogger logger, Exception exception, IEnumerable<string> state, Type projectionType);
}
