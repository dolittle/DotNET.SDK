// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Processing;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Events.Processing"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Warning, "{Kind} {Identifier} failed to connect to the Runtime, retrying in 1s")]
    internal static partial void FailedToConnectToRuntime(this ILogger logger, EventProcessorKind kind, ConceptAs<Guid> identifier);
    
    [LoggerMessage(0, LogLevel.Warning, "{Kind} {Identifier} received a failure from the Runtime, retrying in 1s. {FailureReason}")]
    internal static partial void ReceivedFailureFromRuntime(this ILogger logger, EventProcessorKind kind, ConceptAs<Guid> identifier, string failureReason);
    
    [LoggerMessage(0, LogLevel.Information, "{Kind} {Identifier} registered with the Runtime, start handling requests")]
    internal static partial void Registered(this ILogger logger, EventProcessorKind kind, ConceptAs<Guid> identifier);
    
    [LoggerMessage(0, LogLevel.Warning, "{Kind} {Identifier} has stopped processing, retrying in 1s")]
    internal static partial void StoppedDuringProcessing(this ILogger logger, EventProcessorKind kind, ConceptAs<Guid> identifier);
    
    [LoggerMessage(0, LogLevel.Warning, "{Kind} {Identifier} registration failed with exception, retrying in 1s")]
    internal static partial void RegistrationFailed(this ILogger logger, EventProcessorKind kind, ConceptAs<Guid> identifier, Exception exception);
    
    [LoggerMessage(0, LogLevel.Debug, "{Kind} {identifier} handling of requests stopped")]
    internal static partial void ProcessingCompleted(this ILogger logger, EventProcessorKind kind, ConceptAs<Guid> identifier);
}
