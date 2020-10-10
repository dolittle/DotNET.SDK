// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct an <see cref="UncommittedEvent"/> with content that is null.
    /// </summary>
    public class EventContentCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventContentCannotBeNull"/> class.
        /// </summary>
        public EventContentCannotBeNull()
            : base($"The content of an {nameof(UncommittedEvent)} cannot be null")
        {
        }
    }
}