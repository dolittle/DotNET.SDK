// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events
{
    /// <summary>
    /// Exception that gets thrown when an inconsistency is detected in the event store at runtime.
    /// </summary>
    public class EventStoreConsistencyError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreConsistencyError"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public EventStoreConsistencyError(string reason)
            : base(reason)
        {
        }
    }
}
