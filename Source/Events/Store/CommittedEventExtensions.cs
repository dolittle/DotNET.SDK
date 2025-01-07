// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Extension methods for <see cref="CommittedEvent"/>.
/// </summary>
public static class CommittedEventExtensions
{
    /// <summary>
    /// Gets the <see cref="EventContext"/> for a <see cref="CommittedEvent"/>.
    /// </summary>
    /// <param name="event">The <see cref="CommittedEvent"/> to get the context for.</param>
    /// <param name="currentExecutionContext">The <see cref="ExecutionContext"/> in which the event is currently being processed.</param>
    /// <param name="streamPosition">Optionally, the <see cref="StreamPosition"/> of the event in the stream.</param>
    /// <returns>The <see cref="EventContext"/> for a <see cref="CommittedEvent"/>.</returns>
    public static EventContext GetEventContext(this CommittedEvent @event, ExecutionContext currentExecutionContext, StreamPosition? streamPosition)
        => new(
            @event.EventLogSequenceNumber,
            @event.EventType,
            @event.EventSource,
            @event.Occurred,
            @event.ExecutionContext,
            currentExecutionContext,
            streamPosition);
}
