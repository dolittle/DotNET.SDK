// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Internal;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Events.Internal"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Registering Event Type {EventType} with Alias {Alias}")]
    internal static partial void Registering(this ILogger logger, EventTypeId eventType, IdentifierAlias alias);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to register Event Type {EventType} with Alias {Alias} because {Reason}")]
    internal static partial void FailedToRegister(this ILogger logger, EventTypeId eventType, IdentifierAlias alias, string reason);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while registering Event Type {EventType} with Alias {Alias}")]
    internal static partial void ErrorWhileRegistering(this ILogger logger, EventTypeId eventType, IdentifierAlias alias, Exception exception);
}
