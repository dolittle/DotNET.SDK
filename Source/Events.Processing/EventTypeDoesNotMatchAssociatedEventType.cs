// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when converting event for processing to a clr event and the <see cref="EventType" /> from the processing request does not match the associated <see cref="EventType" />.
    /// </summary>
    public class EventTypeDoesNotMatchAssociatedEventType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeDoesNotMatchAssociatedEventType"/> class.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> from the proceessing request.</param>
        /// <param name="associatedEventType">The associated <see cref="EventType" />.</param>
        /// <param name="type">The <see cref="Type" /> of the event.</param>
        public EventTypeDoesNotMatchAssociatedEventType(EventType eventType, EventType associatedEventType, Type type)
            : base($"Trying to convert event for processing to {type}, but {eventType} in the processing request does not match the associated artifact {associatedEventType}")
        {
        }
    }
}
