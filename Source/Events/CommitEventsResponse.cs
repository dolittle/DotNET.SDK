// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents a response to committed events.
    /// </summary>
    public class CommitEventsResponse : Value<CommitEventsResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitEventsResponse"/> class.
        /// </summary>
        /// <param name="failure"><see cref="Failure"/>.</param>
        /// <param name="committedEvents"><see cref="CommittedEvents"/>.</param>
        public CommitEventsResponse(Failure failure, CommittedEvents committedEvents)
        {
            Failure = failure;
            Events = committedEvents;
            Failed = Failure != null;
        }

        /// <summary>
        /// Gets the <see cref="Failure" />.
        /// </summary>
        public Failure Failure { get; }

        /// <summary>
        /// Gets the <see cref="CommittedEvents" />.
        /// </summary>
        public CommittedEvents Events { get; }

        /// <summary>
        /// Gets a value indicating whether the commit failed.
        /// </summary>
        public bool Failed { get; }
    }
}
