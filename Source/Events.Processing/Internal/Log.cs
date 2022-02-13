// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Processing.Internal;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Events.Processing.Internal"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Warning, "Processing in {Kind} {Identifier} failed. Will retry in {RetrySeconds}")]
    internal static partial void ProcessingFailed(this ILogger logger, EventProcessorKind kind, ConceptAs<Guid> identifier, uint retrySeconds, Exception exception);
}
