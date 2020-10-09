// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using PbCommittedAggregateEventsResponse = Dolittle.Runtime.Events.Contracts.CommitAggregateEventsResponse;
using PbFetchForAggregateResponse = Dolittle.Runtime.Events.Contracts.FetchForAggregateResponse;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Defines a system that is capable of converting Aggregate responses to SDK.
    /// </summary>
    public interface IConvertAggregateResponsesToSDK
    {
        /// <summary>
        /// Convert from <see cref="PbCommittedAggregateEventsResponse"/> to <see cref="CommittedAggregateEvents"/>.
        /// </summary>
        /// <param name="source"><see cref="PbCommittedAggregateEventsResponse"/>.</param>
        /// <param name="events">When the method returns, the converted <see cref="CommittedAggregateEvents"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryToSDK(PbCommittedAggregateEventsResponse source, out CommittedAggregateEvents events, out Exception error);

        /// <summary>
        /// Convert from <see cref="PbFetchForAggregateResponse"/> to <see cref="CommittedAggregateEvents"/>.
        /// </summary>
        /// <param name="source"><see cref="PbFetchForAggregateResponse"/>.</param>
        /// <param name="events">When the method returns, the converted <see cref="CommittedAggregateEvents"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryToSDK(PbFetchForAggregateResponse source, out CommittedAggregateEvents events, out Exception error);
    }
}
