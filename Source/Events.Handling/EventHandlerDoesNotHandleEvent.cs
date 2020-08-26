// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Exception that gets thrown when an event handler is asked to handle an event it cannot handle.
    /// </summary>
    public class EventHandlerDoesNotHandleEvent : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerDoesNotHandleEvent"/> class.
        /// </summary>
        /// <param name="type">The type of the event the event handler was asked to handle.</param>
        public EventHandlerDoesNotHandleEvent(Type type)
            : base($"Event handler cannot handle event of type {type}")
        {
        }
    }
}