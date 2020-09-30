// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when an event type <see cref="Type" /> is not decorated with <see cref="EventTypeAttribute" />.
    /// </summary>
    public class EventTypeNotDecorated : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeNotDecorated"/> class.
        /// </summary>
        /// <param name="type">The event type <see cref="Type" />.</param>
        public EventTypeNotDecorated(Type type)
            : base($"Event type {type} is not decorated with [{typeof(EventTypeAttribute).Name}]")
        {
        }
    }
}