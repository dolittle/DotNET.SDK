// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Decorates a class to indicate the <see cref="EventType" /> of an event class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeAttribute"/> class.
        /// </summary>
        /// <param name="eventTypeId">The unique identifier of the <see cref="EventType" />.</param>
        /// <param name="generation">The generation of the <see cref="EventType" />..</param>
        public EventTypeAttribute(string eventTypeId, uint generation = 0)
            => EventType = new EventType(
                Guid.Parse(eventTypeId),
                generation == 0 ? Generation.First : new Generation { Value = generation });

        /// <summary>
        /// Gets the <see cref="Events.EventType" />.
        /// </summary>
        public EventType EventType { get; }
    }
}
