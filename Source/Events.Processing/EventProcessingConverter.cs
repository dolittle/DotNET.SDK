// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Protobuf;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using PbStreamEvent = Dolittle.Runtime.Events.Processing.Contracts.StreamEvent;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// An implementation of <see cref="IEventProcessingConverter"/>.
    /// </summary>
    public class EventProcessingConverter : IEventProcessingConverter
    {
        readonly IEventConverter _eventConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessingConverter"/> class.
        /// </summary>
        /// <param name="eventConverter">The <see cref="IEventConverter"/> to use for converting to <see cref="CommittedEvent"/>.</param>
        public EventProcessingConverter(IEventConverter eventConverter)
        {
            _eventConverter = eventConverter;
        }

        /// <inheritdoc/>
        public CommittedEvent ToSDK(PbCommittedEvent source)
            => _eventConverter.ToSDK(source);

        /// <inheritdoc/>
        public StreamEvent ToSDK(PbStreamEvent source)
            => new StreamEvent(
                ToSDK(source.Event),
                source.Partitioned,
                source.PartitionId.To<PartitionId>(),
                source.ScopeId.To<ScopeId>());
    }
}