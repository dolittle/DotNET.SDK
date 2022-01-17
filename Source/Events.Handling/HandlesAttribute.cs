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
                generation == 0 ? Generation.First : new(generation));

        /// <summary>
        /// Gets the <see cref="Events.EventType" />.
        /// </summary>
        public EventType EventType { get; }
    }
}
