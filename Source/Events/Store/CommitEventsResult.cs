// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;
using Dolittle.SDK.Protobuf;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represents a response to committed events.
    /// </summary>
    public class CommitEventsResult : Value<CommitEventsResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitEventsResult"/> class.
        /// </summary>
        /// <param name="failure"><see cref="Failure"/>.</param>
        /// <param name="committedEvents"><see cref="CommittedEvents"/>.</param>
        public CommitEventsResult(Failure failure, CommittedEvents committedEvents)
        {
            Failure = failure;
            Events = committedEvents;
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
        public bool Failed => Failure != null;
    }
}
