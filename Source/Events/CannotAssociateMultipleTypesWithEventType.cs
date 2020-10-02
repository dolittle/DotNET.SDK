// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when attempting to associate multiple instance of <see cref="Type"/> with a single <see cref="EventType"/>.
    /// </summary>
    public class CannotAssociateMultipleTypesWithEventType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAssociateMultipleTypesWithEventType"/> class.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType"/> that was attempted to associate with a <see cref="Type"/>.</param>
        /// <param name="type">The <see cref="Type"/> that was attempted to associate with.</param>
        /// <param name="existing">The <see cref="Type"/> that the <see cref="EventType"/> was already associated with.</param>
        public CannotAssociateMultipleTypesWithEventType(EventType eventType, Type type, Type existing)
            : base($"{eventType} cannot be associated with {type} because it is already associated with {existing}")
        {
        }
    }
}
