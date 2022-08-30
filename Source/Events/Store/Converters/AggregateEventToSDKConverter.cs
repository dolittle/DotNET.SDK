// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Protobuf;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Represents an implementation of <see cref="IConvertAggregateEventsToSDK"/>.
/// </summary>
public class AggregateEventToSDKConverter : IConvertAggregateEventsToSDK
{
    readonly ISerializeEventContent _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateEventToSDKConverter"/> class.
    /// </summary>
    /// <param name="serializer"><see cref="ISerializeEventContent"/> for deserializing event contents.</param>
    public AggregateEventToSDKConverter(ISerializeEventContent serializer)
    {
        _serializer = serializer;
    }

    /// <inheritdoc/>
    public bool TryConvert(Runtime.Events.Contracts.CommittedAggregateEvents source, out CommittedAggregateEvents events, out Exception error)
    {
        events = default;

        if (source == null)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        if (!source.AggregateRootId.TryTo<AggregateRootId>(out var aggregateRootId, out var aggregateRootIdError))
        {
            error = new InvalidCommittedEventInformation(nameof(source.AggregateRootId), aggregateRootIdError);
            return false;
        }

        if (!TryConvert(source.Events, source.EventSourceId, aggregateRootId, source.AggregateRootVersion, out var committedAggregateEventList, out error))
        {
            return false;
        }

        events = new CommittedAggregateEvents(source.EventSourceId, aggregateRootId, committedAggregateEventList);
        error = null;
        return true;
    }

    bool TryConvert(
        IEnumerable<Runtime.Events.Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent> source,
        EventSourceId eventSourceId,
        AggregateRootId aggregateRootId,
        AggregateRootVersion aggregateRootVersion,
        out List<CommittedAggregateEvent> events,
        out Exception error)
    {
        events = default;
        var list = new List<CommittedAggregateEvent>();
        var committedEvents = source.ToArray();
        for (ulong i = 0; i < (ulong)committedEvents.Length; i++)
        {
            // We have to manually calculate and set the AggregateRootVersion for the events as the
            // CommittedAggregateEvents.AggregateRootVersion is set to the latest version.
            var version = aggregateRootVersion + 1u - (ulong)committedEvents.Length + i;
            var sourceEvent = committedEvents[i];

            if (!TryConvert(sourceEvent, eventSourceId, aggregateRootId, version, out var @event, out error))
            {
                return false;
            }

            list.Add(@event);
        }

        events = list;
        error = null;
        return true;
    }

    bool TryConvert(
        Runtime.Events.Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent source,
        EventSourceId eventSourceId,
        AggregateRootId aggregateRootId,
        AggregateRootVersion aggregateRootVersion,
        out CommittedAggregateEvent result,
        out Exception error)
    {
        result = default;

        if (source == null)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        if (!source.EventType.TryToArtifact<EventTypeId>(out var eventTypeId, out var generation, out var eventTypeError))
        {
            error = new InvalidCommittedEventInformation(nameof(source.EventType), eventTypeError);
            return false;
        }

        if (!source.ExecutionContext.TryToExecutionContext(out var executionContext, out var executionContextError))
        {
            error = new InvalidCommittedEventInformation(nameof(source.ExecutionContext), executionContextError);
            return false;
        }

        if (source.Occurred == null)
        {
            error = new MissingCommittedEventInformation(nameof(source.Occurred));
            return false;
        }

        if (string.IsNullOrWhiteSpace(source.Content))
        {
            error = new MissingCommittedEventInformation(nameof(source.Content));
            return false;
        }

        var eventType = new EventType(eventTypeId, generation);
        if (!_serializer.TryDeserialize(eventType, source.EventLogSequenceNumber, source.Content, out var content, out var deserializationError))
        {
            error = new InvalidCommittedEventInformation(nameof(source.Content), deserializationError);
            return false;
        }

        error = null;
        result = new CommittedAggregateEvent(
            source.EventLogSequenceNumber,
            source.Occurred.ToDateTimeOffset(),
            eventSourceId,
            aggregateRootId,
            aggregateRootVersion,
            executionContext,
            eventType,
            content,
            source.Public);
        return true;
    }
}
