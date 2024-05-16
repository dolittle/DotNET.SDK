// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Internal;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Projections.Store"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Error, "Error processing projection for {@CommittedEvent} ")]
    internal static partial void ErrorProcessingProjectionEvent(this ILogger logger, Exception e, CommittedEvent committedEvent);
}
