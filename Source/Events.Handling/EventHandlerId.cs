// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Represents the concept of an event handler.
    /// </summary>
    public class EventHandlerId : ConceptAs<Guid>
    {
        /// <summary>
        /// Gets the value for a unset <see cref="EventHandlerId"/>.
        /// </summary>
        public static readonly EventHandlerId NotSet = Guid.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerId"/> class.
        /// </summary>
        public EventHandlerId()
        {
            Value = Guid.Empty;
        }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="EventHandlerId"/> has a value or not.
        /// </summary>
        public bool IsNotSet => Value == NotSet;

        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> to convert from.</param>
        /// <returns><see cref="EventHandlerId"/> instance.</returns>
        public static implicit operator EventHandlerId(Guid id) => new EventHandlerId { Value = id };
    }
}