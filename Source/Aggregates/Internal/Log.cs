// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Aggregates.Internal"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting aggregate root {AggregateRoot} with event source id {EventSource}")]
    internal static partial void Get(this ILogger logger, Type aggregateRoot, EventSourceId eventSource);

    [LoggerMessage(0, LogLevel.Debug, "Registering Aggregate Root {AggregateRoot} with Alias {Alias}")]
    internal static partial void Registering(this ILogger logger, AggregateRootId aggregateRoot, IdentifierAlias alias);

    [LoggerMessage(0, LogLevel.Warning, "A failure occurred while registering Aggregate Root {AggregateRoot} with Alias {Alias} because {Reason}")]
    internal static partial void FailedToRegister(this ILogger logger, AggregateRootId aggregateRoot, IdentifierAlias alias, string reason);
    
    [LoggerMessage(0, LogLevel.Warning,"An error occurred while registering Aggregate Root {AggregateRoot} with Alias {Alias}" )]
    internal static partial void ErrorDuringRegister(this ILogger logger, AggregateRootId aggregateRoot, IdentifierAlias alias, Exception exception);
}
