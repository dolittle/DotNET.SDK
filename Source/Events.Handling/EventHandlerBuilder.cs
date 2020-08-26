// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Handling.Builder;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Static methods for creating event handler builders.
    /// </summary>
    public static class EventHandlerBuilder
    {
        /// <summary>
        /// Creates a new <see cref="EventHandlerBuilder{TEventType}"/> that can be used to build <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/> from delegates.
        /// </summary>
        /// <typeparam name="TEventType">The type of events to handle.</typeparam>
        /// <returns>A <see cref="EventHandlerBuilder{TEventType}"/> that can be used to build <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/> from delegates.</returns>
        public static EventHandlerBuilder<TEventType> Create<TEventType>()
            where TEventType : IEvent
            => new EventHandlerBuilder<TEventType>();
    }
}
