// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when attempting to associate multiple instance of <see cref="EventType"/> with a single <see cref="Type"/>.
    /// </summary>
    public class CannotAssociateMultipleEventTypesWithType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAssociateMultipleEventTypesWithType"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that was attempted to associate with a <see cref="EventType"/>.</param>
        /// <param name="eventType">The <see cref="EventType"/> that was attempted to associate with.</param>
        /// <param name="existing">The <see cref="EventType"/> that the <see cref="Type"/> was already associated with.</param>
        public CannotAssociateMultipleEventTypesWithType(Type type, EventType eventType, EventType existing)
            : base($"{type} cannot be associated with {eventType} because it is already associated with {existing}")
        {
        }
    }
}
