// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Represents the concept of a unique identifier for an event handler.
    /// </summary>
    public class EventHandlerId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="EventHandlerId"/>.</returns>
        public static implicit operator EventHandlerId(Guid id) => new EventHandlerId { Value = id };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="EventHandlerId"/>.</returns>
        public static implicit operator EventHandlerId(string id) => new EventHandlerId { Value = Guid.Parse(id) };
    }
}