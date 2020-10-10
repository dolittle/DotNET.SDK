// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct an <see cref="UncommittedEvent"/> or <see cref="UncommittedAggregateEvents"/> without an <see cref="EventSourceId"/>.
    /// </summary>
    public class EventSourceIdCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceIdCannotBeNull"/> class.
        /// </summary>
        public EventSourceIdCannotBeNull()
            : base($"The {nameof(EventSourceId)} of an {nameof(UncommittedEvent)} or {nameof(UncommittedAggregateEvents)} cannot be null")
        {
        }
    }
}