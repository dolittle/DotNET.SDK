// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when trying to construct an <see cref="EventType"/> without an <see cref="EventTypeId"/>.
    /// </summary>
    public class EventTypeIdCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeIdCannotBeNull"/> class.
        /// </summary>
        public EventTypeIdCannotBeNull()
            : base($"The {nameof(EventTypeId)} of an {nameof(EventType)} cannot be null")
        {
        }
    }
}