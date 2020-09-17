// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        /// <param name="event">When the method returns, the converted <see cref="CommittedEvent"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryToSDK(PbCommittedEvent source, out CommittedEvent @event, out Exception error);

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
        /// <param name="events">When the method returns, the converted <see cref="CommittedEvents"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryToSDK(IEnumerable<PbCommittedEvent> source, out CommittedEvents events, out Exception error);

        /// <summary>
        /// Convert from <see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/> to <see cref="CommittedEvents"/>.
        /// </summary>
        /// <param name="source"><see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/>.</param>
        /// <returns>Converted <see cref="CommittedEvents"/>.</returns>
        CommittedEvents ToSDK(IEnumerable<PbCommittedEvent> source);

        /// <summary>
        /// Convert from <see cref="PbCommitEventsResponse"/> to <see cref="CommitEventsResult"/>.
        /// </summary>
        /// <param name="source"><see cref="PbCommitEventsResponse"/>.</param>
        /// <param name="result">When the method returns, the converted <see cref="CommitEventsResult"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryToSDK(PbCommitEventsResponse source, out CommitEventsResult result, out Exception error);

        /// <summary>
        /// Convert from <see cref="PbCommitEventsResponse"/> to <see cref="CommitEventsResult"/>.
        /// </summary>
        /// <param name="source"><see cref="PbCommitEventsResponse"/>.</param>
        /// <returns>Converted <see cref="CommitEventsResult"/>.</returns>
        CommitEventsResult ToSDK(PbCommitEventsResponse source);

        /// <summary>
        /// Convert from <see cref="UncommittedEvent" /> to <see cref="PbUncommittedEvent" />.
        /// </summary>
        /// <param name="source"><see cref="UncommittedEvent"/>.</param>
        /// <param name="event">When the method returns, the converted <see cref="PbUncommittedEvent"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryToProtobuf(UncommittedEvent source, out PbUncommittedEvent @event, out Exception error);

        /// <summary>
        /// Convert from <see cref="UncommittedEvent" /> to <see cref="PbUncommittedEvent" />.
        /// </summary>
        /// <param name="source"><see cref="UncommittedEvent" />.</param>
        /// <returns>Converted <see cref="PbUncommittedEvent" />.</returns>
        PbUncommittedEvent ToProtobuf(UncommittedEvent source);

        /// <summary>
        /// Convert from <see cref="UncommittedEvents" /> to <see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/>.
        /// </summary>
        /// <param name="source"><see cref="UncommittedEvents"/>.</param>
        /// <param name="events">When the method returns, the converted <see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/>. if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryToProtobuf(UncommittedEvents source, out IEnumerable<PbUncommittedEvent> events, out Exception error);

        /// <summary>
        /// Convert from <see cref="UncommittedEvent" /> to <see cref="IEnumerable{T}"/> of type <see cref="PbUncommittedEvent"/>.
        /// </summary>
        /// <param name="source"><see cref="UncommittedEvent" />.</param>
        /// <returns>Converted see <see cref="IEnumerable{T}"/> of type <see cref="PbUncommittedEvent"/>.</returns>
        IEnumerable<PbUncommittedEvent> ToProtobuf(UncommittedEvents source);
    }
}
