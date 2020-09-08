// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IEventStore" /> operation failed.
    /// </summary>
    public class EventStoreOperationFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreOperationFailed"/> class.
        /// </summary>
        /// <param name="reason">The reason why the operation failed.</param>
        public EventStoreOperationFailed(string reason)
            : base($"Event Store operation failed because: {reason}")
        {
        }
    }
}
