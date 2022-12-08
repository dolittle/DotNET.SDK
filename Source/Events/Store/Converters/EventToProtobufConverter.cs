// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Dolittle.SDK.Protobuf;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Represents an implementation of <see cref="IConvertEventsToProtobuf"/>.
/// </summary>
public class EventToProtobufConverter : IConvertEventsToProtobuf
{
    readonly ISerializeEventContent _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventToProtobufConverter"/> class.
    /// </summary>
    /// <param name="serializer">The <see cref="ISerializeEventContent"/> serializer.</param>
    public EventToProtobufConverter(ISerializeEventContent serializer)
    {
        _serializer = serializer;
    }

    /// <inheritdoc/>
    public bool TryConvert(UncommittedEvents? source, [NotNullWhen(true)] out IReadOnlyList<Runtime.Events.Contracts.UncommittedEvent>? events, [NotNullWhen(false)] out Exception? error)
    {
        events = default;

        if (source == null)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        var list = ImmutableList<Runtime.Events.Contracts.UncommittedEvent>.Empty.ToBuilder();
        foreach (var sourceEvent in source)
        {
            if (!TryConvert(sourceEvent, out var @event, out error))
            {
                return false;
            }

            list.Add(@event);
        }

        error = null;
        events = list.ToImmutable();
        return true;
    }

    bool TryConvert(UncommittedEvent? source, [NotNullWhen(true)] out Runtime.Events.Contracts.UncommittedEvent? @event, [NotNullWhen(false)] out Exception? error)
    {
        @event = default;

        if (source == null)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        if (!_serializer.TrySerialize(source.Content, out var content, out error))
        {
            return false;
        }

        error = null;
        @event = new Runtime.Events.Contracts.UncommittedEvent
        {
            EventType = source.EventType.ToProtobuf(),
            EventSourceId = source.EventSource.Value,
            Public = source.IsPublic,
            Content = content,
        };
        return true;
    }
}
