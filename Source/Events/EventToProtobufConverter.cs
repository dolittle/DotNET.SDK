// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Protobuf;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Reperesents an implementation of <see cref="IConvertEventsToProtobuf"/>.
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
        public bool TryToProtobuf(UncommittedEvents source, out IEnumerable<Contracts.UncommittedEvent> events, out Exception error)
        {
            events = default;

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            var list = new List<Contracts.UncommittedEvent>();
            foreach (var sourceEvent in source)
            {
                if (!TryToProtobuf(sourceEvent, out var @event, out error))
                    return false;

                list.Add(@event);
            }

            error = null;
            events = list;
            return true;
        }

        bool TryToProtobuf(UncommittedEvent source, out Contracts.UncommittedEvent @event, out Exception error)
        {
            @event = default;

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            if (!_serializer.TryToSerialize(source.Content, out var content, out var serializationError))
            {
                error = serializationError;
                return false;
            }

            error = null;
            @event = new Contracts.UncommittedEvent
            {
                Artifact = source.EventType.ToProtobuf(),
                EventSourceId = source.EventSource.ToProtobuf(),
                Public = source.IsPublic,
                Content = content,
            };
            return true;
        }
    }
}
