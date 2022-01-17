// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Decorates a method to indicate the <see cref="EventType" /> it should be called with.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnAttribute"/> class.
        /// </summary>
        /// <param name="eventTypeId">The unique identifier of the <see cref="EventType" />.</param>
        /// <param name="generation">The generation of the <see cref="EventType" />..</param>
        public OnAttribute(string eventTypeId, uint generation = 0)
            => EventType = new EventType(
                Guid.Parse(eventTypeId),
                generation == 0 ? Generation.First : new(generation));

        /// <summary>
        /// Gets the <see cref="Events.EventType" />.
        /// </summary>
        public EventType EventType { get; }
    }
}
