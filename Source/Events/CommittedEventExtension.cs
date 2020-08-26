// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Events
{
    /// <summary>
    /// Extension methods for <see cref="CommittedEvent"/>.
    /// </summary>
    public static class CommittedEventExtension
    {
        /// <summary>
        /// Derives the <see cref="EventContext"/> for a <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <returns>The derived <see cref="EventContext"/>.</returns>
        public static EventContext DeriveContext(this CommittedEvent committedEvent)
        {
            if (committedEvent is CommittedExternalEvent externalEvent)
            {
                return externalEvent.DeriveContext();
            }
            else
            {
                return new EventContext(
                    committedEvent.EventLogSequenceNumber,
                    committedEvent.EventSource,
                    committedEvent.Occurred,
                    committedEvent.ExecutionContext);
            }
        }

        /// <summary>
        /// Derives the <see cref="ExternalEventContext"/> for a <see cref="CommittedExternalEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedExternalEvent"/>.</param>
        /// <returns>The derived <see cref="ExternalEventContext"/>.</returns>
        public static ExternalEventContext DeriveContext(this CommittedExternalEvent committedEvent) =>
            new ExternalEventContext(
                committedEvent.EventLogSequenceNumber,
                committedEvent.EventSource,
                committedEvent.Occurred,
                committedEvent.ExecutionContext,
                committedEvent.ExternalEventLogSequenceNumber,
                committedEvent.Received);
    }
}