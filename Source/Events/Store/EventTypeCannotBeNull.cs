// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct an <see cref="UncommittedEvent"/> without an <see cref="EventType"/>.
    /// </summary>
    public class EventTypeCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeCannotBeNull"/> class.
        /// </summary>
        public EventTypeCannotBeNull()
            : base($"The {nameof(EventType)} of an {nameof(UncommittedEvent)} cannot be null")
        {
        }
    }
}