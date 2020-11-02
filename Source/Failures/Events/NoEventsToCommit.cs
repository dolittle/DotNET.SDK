// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events
{
    /// <summary>
    /// Exception that gets thrown when there are no events in the uncommitted events sequence when it is being committed.
    /// </summary>
    public class NoEventsToCommit : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventsToCommit"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public NoEventsToCommit(string reason)
            : base(reason)
        {
        }
    }
}