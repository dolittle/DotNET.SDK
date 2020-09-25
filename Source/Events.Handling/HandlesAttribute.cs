// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Decorates a method to indicate the <see cref="EventType" /> that it handles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandlesAttribute"/> class.
        /// </summary>
        /// <param name="eventTypeId">The unique identifier of the <see cref="EventType" />.</param>
        /// <param name="generation">The generation of the <see cref="EventType" />..</param>
        public HandlesAttribute(string eventTypeId, uint generation = 0)
            => EventType = new EventType(
                Guid.Parse(eventTypeId),
                generation == 0 ? Generation.First : new Generation { Value = generation });

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlesAttribute"/> class.
        /// </summary>
        /// <param name="eventClrType">The <see cref="Type" /> of the event.</param>
        public HandlesAttribute(Type eventClrType)
            => EventClrType = eventClrType;

        /// <summary>
        /// Gets the <see cref="Events.EventType" />.
        /// </summary>
        public EventType EventType { get; }

        /// <summary>
        /// Gets the <see cref="Type" /> of the event.
        /// </summary>
        public Type EventClrType { get; }

        /// <summary>
        /// Try to get the <see cref="EventType" />.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventType">The outputtet <see cref="EventType" />.</param>
        /// <returns>A value indicating whether the <see cref="EventType" /> was found or not.</returns>
        public bool TryGetEventType(IEventTypes eventTypes, out EventType eventType)
        {
            eventType = EventType;
            if (eventType != default) return true;
            if (!eventTypes.HasFor(EventClrType)) return false;
            eventType = eventTypes.GetFor(EventClrType);
            return true;
        }

        /// <summary>
        /// Try to get the <see cref="Type" />.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventClrType">The outputtet <see cref="Type" />.</param>
        /// <returns>A value indicating whether the <see cref="Type" /> event clr type was found or not.</returns>
        public bool TryGetEventClrType(IEventTypes eventTypes, out Type eventClrType)
        {
            eventClrType = EventClrType;
            if (eventClrType != default) return true;
            if (!eventTypes.HasTypeFor(EventType)) return false;
            eventClrType = eventTypes.GetTypeFor(EventType);
            return true;
        }
    }
}
