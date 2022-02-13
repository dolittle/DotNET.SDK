// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Connecting Dolittle Client")]
    internal static partial void ConnectingDolittleClient(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Error, "An error occurred while connecting Dolittle Client")]
    internal static partial void ErrorWhileConnectingDolittleClient(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Information, "Disconnecting Dolittle Client")]
    internal static partial void DisconnectingDolittleClient(this ILogger logger);
}
