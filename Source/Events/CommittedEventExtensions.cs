// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Extension methods for <see cref="CommittedEvent"/>.
    /// </summary>
    public static class CommittedEventExtensions
    {
        /// <summary>
        /// Gets the <see cref="EventContext"/> for a <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent"/> to get the context for.</param>
        /// <returns>The <see cref="EventContext"/> for a <see cref="CommittedEvent"/>.</returns>
        public static EventContext GetEventContext(this CommittedEvent @event)
            => new EventContext(
                @event.EventLogSequenceNumber,
                @event.EventSource,
                @event.Occurred,
                @event.ExecutionContext);
    }
}
