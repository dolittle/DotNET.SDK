// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Handshake"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Connecting to Dolittle Runtime")]
    internal static partial void ConnectingToDolittleRuntime(this ILogger logger);

    [LoggerMessage(0, LogLevel.Debug, "Failed connecting to Dolittle Runtime. Retrying in {RetryTime}. {FailureReason}")]
    internal static partial void RetryConnect(this ILogger logger, TimeSpan retryTime, string failureReason);
}
