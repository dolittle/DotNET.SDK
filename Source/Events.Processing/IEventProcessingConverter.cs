// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using PbStreamEvent = Dolittle.Runtime.Events.Processing.Contracts.StreamEvent;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// Defines a system that is capable of converting events for processing from protobuf.
    /// </summary>
    public interface IEventProcessingConverter
    {
        /// <summary>
        /// Convert from <see cref="PbCommittedEvent"/> to <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="source"><see cref="PbCommittedEvent"/>.</param>
        /// <returns>Converted <see cref="CommittedEvent"/>.</returns>
        CommittedEvent ToSDK(PbCommittedEvent source);

        /// <summary>
        /// Convert from <see cref="PbStreamEvent"/> to <see cref="StreamEvent"/>.
        /// </summary>
        /// <param name="source"><see cref="PbStreamEvent"/>.</param>
        /// <returns>Converted <see cref="StreamEvent"/>.</returns>
        StreamEvent ToSDK(PbStreamEvent source);
    }
}