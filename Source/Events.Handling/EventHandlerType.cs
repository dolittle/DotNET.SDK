// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Represents the type of an event handler.
    /// </summary>
    public class EventHandlerType : Value<EventHandlerType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerType"/> class.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandlerId" />.</param>
        /// <param name="type">The <see cref="Type" /> of the event handler.</param>
        public EventHandlerType(EventHandlerId handler, Type type)
        {
            EventHandler = handler;
            Type = type;
        }

        /// <summary>
        /// Gets the <see cref="Type" /> of the event handler.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="EventHandlerId" />.
        /// </summary>
        public EventHandlerId EventHandler { get; }
    }
}
