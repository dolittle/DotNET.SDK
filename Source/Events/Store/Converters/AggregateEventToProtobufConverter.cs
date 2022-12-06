// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Dolittle.SDK.Protobuf;
using PbUncommittedAggregateEvent = Dolittle.Runtime.Events.Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent;
using PbUncommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.UncommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Reperesents an implementation of <see cref="IConvertAggregateEventsToProtobuf"/>.
/// </summary>
public class AggregateEventToProtobufConverter : IConvertAggregateEventsToProtobuf
{
    readonly ISerializeEventContent _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateEventToProtobufConverter"/> class.
    /// </summary>
    /// <param name="serializer">The <see cref="ISerializeEventContent"/> serializer.</param>
    public AggregateEventToProtobufConverter(ISerializeEventContent serializer)
    {
        _serializer = serializer;
    }

    /// <inheritdoc/>
    public bool TryConvert(UncommittedAggregateEvents? source, [NotNullWhen(true)] out PbUncommittedAggregateEvents? events, [NotNullWhen(false)] out Exception? error)
    {
        events = default;

        if (source == null)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        events = new PbUncommittedAggregateEvents
        {
            EventSourceId = source.EventSource.Value,
            AggregateRootId = source.AggregateRoot.ToProtobuf(),
            ExpectedAggregateRootVersion = source.ExpectedAggregateRootVersion,
        };

        foreach (var sourceEvent in source)
        {
            if (!TryConvert(sourceEvent, out var @event, out error))
            {
                events = default;
                return false;
            }

            events.Events.Add(@event);
        }

        error = null;
        return true;
    }

    bool TryConvert(UncommittedAggregateEvent? source, [NotNullWhen(true)] out PbUncommittedAggregateEvent? @event, [NotNullWhen(false)] out Exception? error)
    {
        @event = default;

        if (source == null)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        if (!_serializer.TrySerialize(source.Content, out var content, out error))
            return false;

        error = null;
        @event = new PbUncommittedAggregateEvent
        {
            EventType = source.EventType.ToProtobuf(),
            Content = content,
            Public = source.IsPublic,
        };
        return true;
    }
}
