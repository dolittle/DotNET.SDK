// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Represents a projection event selector.
    /// </summary>
    public class EventSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSelector"/> class.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event.</param>
        /// <param name="keySelector">The <see cref="KeySelector" />.</param>
        public EventSelector(EventType eventType, KeySelector keySelector)
        {
            EventType = eventType;
            KeySelector = keySelector;
        }

        /// <summary>
        /// Gets the <see cref="Events.EventType" />.
        /// </summary>
        public EventType EventType { get; }

        /// <summary>
        /// Gets the <see cref="Projections.KeySelector" />.
        /// </summary>
        public KeySelector KeySelector { get; }
    }
}
