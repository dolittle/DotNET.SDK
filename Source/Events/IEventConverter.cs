// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using PbCommitEventsResponse = Dolittle.Runtime.Events.Contracts.CommitEventsResponse;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Defines a system that is capable of converting events to and from protobuf.
    /// </summary>
    public interface IEventConverter
    {
        /// <summary>
        /// Convert from <see cref="PbCommittedEvent"/> to <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="source"><see cref="PbCommittedEvent"/>.</param>
        /// <returns>Converted <see cref="CommittedEvent"/>.</returns>
        CommittedEvent ToSDK(PbCommittedEvent source);

        /// <summary>
        /// Convert from <see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/> to <see cref="CommittedEvents"/>.
        /// </summary>
        /// <param name="source"><see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/>.</param>
        /// <returns>Converted <see cref="CommittedEvents"/>.</returns>
        CommittedEvents ToSDK(IEnumerable<PbCommittedEvent> source);

        /// <summary>
        /// Convert from <see cref="UncommittedEvent" /> to <see cref="PbUncommittedEvent" />.
        /// </summary>
        /// <param name="event"><see cref="UncommittedEvent" />.</param>
        /// <returns>Converted <see cref="PbUncommittedEvent" />.</returns>
        PbUncommittedEvent ToProtobuf(UncommittedEvent @event);

        /// <summary>
        /// Convert from <see cref="UncommittedEvent" /> to <see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/>.
        /// </summary>
        /// <param name="events"><see cref="UncommittedEvent" />.</param>
        /// <returns>Converted see <see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/>.</returns>
        IEnumerable<PbUncommittedEvent> ToProtobuf(UncommittedEvents events);

        /// <summary>
        /// Convert from <see cref="PbCommitEventsResponse"/> to <see cref="CommitEventsResult"/>.
        /// </summary>
        /// <param name="source"><see cref="PbCommitEventsResponse"/>.</param>
        /// <returns>Converted <see cref="CommitEventsResult"/>.</returns>
        CommitEventsResult ToSDK(PbCommitEventsResponse source);
    }
}
