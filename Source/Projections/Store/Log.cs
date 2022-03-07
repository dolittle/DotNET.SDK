// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Projections.Store"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting read model of projection {Projection} of type {ProjectionType} in scope {Scope} with key {Key}")]
    internal static partial void GettingOneProjection(this ILogger logger, Key key, ProjectionId projection, Type projectionType, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Debug, "Getting the state of projection {Projection} of type {ProjectionType} in scope {Scope} with key {Key}")]
    internal static partial void GettingOneProjectionState(this ILogger logger, Key key, ProjectionId projection, Type projectionType, ScopeId scope);

    [LoggerMessage(0, LogLevel.Debug, "Getting all read models from projection {Projection} of type {ProjectionType} in scope {Scope}")]
    internal static partial void GettingAllProjections(this ILogger logger, ProjectionId projection, Type projectionType, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Trace, "Received batch number {BatchNumber} consisting of {NumProjections} projection read models")]
    internal static partial void ProcessingProjectionsInBatch(this ILogger logger, int batchNumber, int numProjections);
    
    [LoggerMessage(0, LogLevel.Error, "Could not convert projection read model to {ProjectionType}")]
    internal static partial void FailedToConvertProjectionState(this ILogger logger, Type projectionType, Exception exception);
}
