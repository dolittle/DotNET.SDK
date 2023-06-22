// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Services;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Services"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Received ping")]
    internal static partial void ReceivedPing(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Writing pong")]
    internal static partial void WritingPong(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Received connect response")]
    internal static partial void ReceivedConnectResponse(this ILogger logger);

    [LoggerMessage(0, LogLevel.Warning, "Received a message from Reverse Call Dispatcher while connecting, but it was not a registration response or a ping" )]
    internal static partial void ReceivedNonPingOrResponseDuringConnect(this ILogger logger);

    [LoggerMessage(0, LogLevel.Warning, "No messages received from the server, connection timed out.")]
    internal static partial void TimedOutDuringConnect(this ILogger logger);

    [LoggerMessage(0, LogLevel.Debug, "Reverse Call Client was cancelled by client while connecting")]
    internal static partial void CancelledByClientDuringConnect(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Reverse Call Client was cancelled by server while connecting")]
    internal static partial void CancelledByServerDuringConnect(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Received message from Reverse Call Dispatcher while handling, but it was not a request or a ping" )]
    internal static partial void ReceivedNonPingOrRequestDuringHandling(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Information, "Received  Disconnect Ack")]
    internal static partial void ReceivedDisconnectAck(this ILogger logger);

    [LoggerMessage(0, LogLevel.Debug, "Reverse Call Client was cancelled by client while handling requests")]
    internal static partial void CancelledByClientDuringHandling(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Reverse Call Client was cancelled by server while handling requests")]
    internal static partial void CancelledByServerDuringHandling(this ILogger logger);

    [LoggerMessage(0, LogLevel.Debug, "Reverse Call Client was cancelled before it could respond with pong")]
    internal static partial void CancelledBeforePongCouldBeSent(this ILogger logger);

    [LoggerMessage(0, LogLevel.Trace, "Handling request with call '{CallId}'")]
    internal static partial void HandlingRequest(this ILogger logger, Guid callId);
    
    [LoggerMessage(0, LogLevel.Warning, "Error while invoking handler for call '{CallId}'")]
    internal static partial void ErrorWhileInvokingHandlerFor(this ILogger logger, Guid callId, Exception exception);
    
    [LoggerMessage(0, LogLevel.Warning, "Error while writing response for call '{CallId}'")]
    internal static partial void ErrorWhileWritingResponseFor(this ILogger logger, Guid callId, Exception exception);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while handling received request")]
    internal static partial void ErrorWhileHandlingRequest(this ILogger logger, Exception exception);
    
    [LoggerMessage(0, LogLevel.Trace, "Writing response for call '{CallId}'")]
    internal static partial void WritingResponseFor(this ILogger logger, Guid callId);
    
    [LoggerMessage(0, LogLevel.Trace, "Client was cancelled while writing response for call '{CallId}'")]
    internal static partial void CancelledWhileWritingResponseFor(this ILogger logger, Guid callId);
}
