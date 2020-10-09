// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        readonly IConvertEventResponsestoSDK _eventResponsestoSDKConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessingConverter"/> class.
        /// </summary>
        /// <param name="convertEventResponsestoSDK">The <see cref="IConvertEventResponsestoSDK"/>.</param>
        public EventProcessingConverter(IConvertEventResponsestoSDK convertEventResponsestoSDK)
        {
            _eventResponsestoSDKConverter = convertEventResponsestoSDK;
        }

        /// <inheritdoc/>
        public bool TryToSDK(PbCommittedEvent source, out CommittedEvent @event, out Exception error)
            => _eventResponsestoSDKConverter.TryToSDK(source, out @event, out error);

        /// <inheritdoc/>
        public CommittedEvent ToSDK(PbCommittedEvent source)
        {
            if (!_eventResponsestoSDKConverter.TryToSDK(source, out var @event, out var error))
                throw error;
            return @event;
        }

        /// <inheritdoc/>
        public bool TryToSDK(PbStreamEvent source, out StreamEvent @event, out Exception error)
        {
            @event = default;

            if (!TryToSDK(source.Event, out var committedEvent, out var eventError))
            {
                error = new InvalidStreamEventInformation(nameof(source.Event), eventError);
                return false;
            }

            if (!source.PartitionId.TryTo<PartitionId>(out var partitionId, out var partitionIdError))
            {
                error = new InvalidStreamEventInformation(nameof(source.PartitionId), partitionIdError);
                return false;
            }

            if (!source.ScopeId.TryTo<ScopeId>(out var scopeId, out var scopeIdError))
            {
                error = new InvalidStreamEventInformation(nameof(source.ScopeId), scopeIdError);
                return false;
            }

            error = null;
            @event = new StreamEvent(
                committedEvent,
                source.Partitioned,
                partitionId,
                scopeId);
            return true;
        }

        /// <inheritdoc/>
        public StreamEvent ToSDK(PbStreamEvent source)
            => TryToSDK(source, out var @event, out var error) ? @event : throw error;
    }
}
