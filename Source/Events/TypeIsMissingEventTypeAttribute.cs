// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when an event type <see cref="Type" /> is missing an <see cref="EventTypeAttribute" />.
    /// </summary>
    public class TypeIsMissingEventTypeAttribute : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeIsMissingEventTypeAttribute"/> class.
        /// </summary>
        /// <param name="type">The event type <see cref="Type" />.</param>
        public TypeIsMissingEventTypeAttribute(Type type)
            : base($"{type} is missing the [EventType(...)] attribute")
        {
        }
    }
}