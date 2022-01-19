// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Connecting Dolittle Client")]
    internal static partial void ConnectingDolittleClient(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Error, "An error occurred while connecting Dolittle Client")]
    internal static partial void ErrorWhileConnectingDolittleClient(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Information, "Disconnecting Dolittle Client")]
    internal static partial void DisconnectingDolittleClient(ILogger logger);
}
