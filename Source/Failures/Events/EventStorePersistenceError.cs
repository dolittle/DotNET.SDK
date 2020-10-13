// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events
{
    /// <summary>
    /// Exception that gets thrown when there is an error when trying to save to the event store.
    /// </summary>
    public class EventStorePersistenceError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStorePersistenceError"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public EventStorePersistenceError(string reason)
            : base(reason)
        {
        }
    }
}