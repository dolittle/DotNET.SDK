// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting one read model from projection {Projection} of type {ProjectionType} in scope {Scope} with key {Key}")]
    internal static partial void GettingOneProjection(ILogger logger, Key key, ProjectionId projection, Type projectionType, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Debug, "Getting one the state of projection {Projection} of type {ProjectionType} in scope {Scope} with key {Key}")]
    internal static partial void GettingOneProjectionState(ILogger logger, Key key, ProjectionId projection, Type projectionType, ScopeId scope);

    [LoggerMessage(0, LogLevel.Debug, "Getting all read models from projection {Projection} of type {ProjectionType} in scope {Scope}")]
    internal static partial void GettingAllProjections(ILogger logger, ProjectionId projection, Type projectionType, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Trace, "Received batch number {BatchNumber} consisting of {NumProjections} projection read models")]
    internal static partial void ProcessingProjectionsInBatch(ILogger logger, int batchNumber, int numProjections);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert projection read model to {ProjectionType}. State: {State}")]
    internal static partial void FailedToConvertProjectionState(ILogger logger, Exception exception, string state, Type projectionType);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert projection read models to {ProjectionType}. States: {State}")]
    internal static partial void FailedToConvertProjectionStates(ILogger logger, Exception exception, IEnumerable<string> state, Type projectionType);
}
