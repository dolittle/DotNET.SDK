// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Represents the concept of a unique identifier for an event handler.
    /// </summary>
    public record EventHandlerId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="EventHandlerId"/>.</returns>
        public static implicit operator EventHandlerId(Guid eventHandlerId) => new(eventHandlerId);

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="EventHandlerId"/>.</returns>
        public static implicit operator EventHandlerId(string eventHandlerId) => new(Guid.Parse(eventHandlerId));
    }
}
