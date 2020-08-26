// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IEvent"/> is null.
    /// </summary>
    public class EventCanNotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventCanNotBeNull"/> class.
        /// </summary>
        public EventCanNotBeNull()
            : base("An event can't be null")
        {
        }
    }
}