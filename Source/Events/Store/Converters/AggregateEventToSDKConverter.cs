// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public bool TryConvert(Runtime.Events.Contracts.CommittedAggregateEvents? source, [NotNullWhen(true)] out CommittedAggregateEvents? events,
        [NotNullWhen(false)] out Exception? error)
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

        if (!TryConvert(source.Events, source.EventSourceId, aggregateRootId, out var committedAggregateEventList, out error))
        {
            return false;
        }

        events = new CommittedAggregateEvents(source.EventSourceId, aggregateRootId, source.CurrentAggregateRootVersion, committedAggregateEventList);
        error = null;
        return true;
    }

    bool TryConvert(
        IEnumerable<Runtime.Events.Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent> source,
        EventSourceId eventSourceId,
        AggregateRootId aggregateRootId,
        [NotNullWhen(true)] out List<CommittedAggregateEvent>? events,
        [NotNullWhen(false)] out Exception? error)
    {
        var list = new List<CommittedAggregateEvent>();
        var committedEvents = source.ToArray();
        for (ulong i = 0; i < (ulong)committedEvents.Length; i++)
        {
            var sourceEvent = committedEvents[i];

            if (!TryConvert(sourceEvent, eventSourceId, aggregateRootId, out var @event, out error))
            {
                events = default;
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
        [NotNullWhen(true)] out CommittedAggregateEvent? result,
        [NotNullWhen(false)] out Exception? error)
    {
        result = default;

        if (source == null)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        if (!source.EventType.TryTo<EventType, EventTypeId>(out var eventType, out var eventTypeError))
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
            source.AggregateRootVersion,
            executionContext,
            eventType,
            content,
            source.Public);
        return true;
    }
}
