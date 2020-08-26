// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Filters.Internal
{
    /// <summary>
    /// Exception that gets thrown when trying to invoke an event filter with an event of incorrect type.
    /// </summary>
    public class EventTypeIsIncorrectForFilter : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeIsIncorrectForFilter"/> class.
        /// </summary>
        /// <param name="expectedType">The <see cref="Type"/> that the event filter accepts.</param>
        /// <param name="eventType">The <see cref="Type"/> of the event to filter.</param>
        public EventTypeIsIncorrectForFilter(Type expectedType, Type eventType)
            : base($"The event filter expects events of type {expectedType}, but was provided an event of type {eventType}")
        {
        }
    }
}