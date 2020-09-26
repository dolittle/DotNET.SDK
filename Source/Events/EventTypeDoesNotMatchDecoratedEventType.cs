// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="EventType" /> is being associated to a <see cref="Type" /> that os decorated with another <see cref="EventType" />.
    /// </summary>
    public class EventTypeDoesNotMatchDecoratedEventType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeDoesNotMatchDecoratedEventType"/> class.
        /// </summary>
        /// <param name="eventTypeToAssociate">The <see cref="EventType" /> that the <see cref="Type" /> is being associated with.</param>
        /// <param name="decoratedEventType">The <see cref="EventType" /> that the <see cref="Type" /> is decorated with.</param>
        /// <param name="type">The <see cref="Type" /> to associate the <see cref="Type" /> to.</param>
        public EventTypeDoesNotMatchDecoratedEventType(EventType eventTypeToAssociate, EventType decoratedEventType, Type type)
            : base($"Attempting to associate {type} with {eventTypeToAssociate} but it is decorated with another event type [{decoratedEventType}]")
        {
        }
    }
}