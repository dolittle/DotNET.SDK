// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Protobuf;
using Newtonsoft.Json;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventConverter"/>.
    /// </summary>
    public class EventConverter : IEventConverter
    {
        readonly IEventTypes _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventConverter"/> class.
        /// </summary>
        /// <param name="eventTypes"><see cref="IEventTypes"/> for mapping types and artifacts.</param>
        public EventConverter(IEventTypes eventTypes)
        {
            _eventTypes = eventTypes;
        }

        /// <inheritdoc/>
        public bool TryToSDK(Contracts.CommittedEvent source, out CommittedEvent @event, out Exception error)
        {
            @event = null;

            if (source.Occurred == null)
            {
                error = new MissingCommittedEventInformation(nameof(source.Occurred));
                return false;
            }

            if (!source.EventSourceId.TryTo<EventSourceId>(out var eventSourceId, out var eventSourceError))
            {
                error = new InvalidCommittedEventInformation(nameof(source.EventSourceId), eventSourceError);
                return false;
            }

            if (!source.ExecutionContext.TryToExecutionContext(out var executionContext, out var executionContextError))
            {
                error = new InvalidCommittedEventInformation(nameof(source.ExecutionContext), executionContextError);
                return false;
            }

            if (!source.Type.TryTo<EventType>(out var eventType, out var eventTypeError))
            {
                error = new InvalidCommittedEventInformation(nameof(source.Type), eventTypeError);
                return false;
            }

            if (string.IsNullOrWhiteSpace(source.Content))
            {
                error = new MissingCommittedEventInformation(nameof(source.Content));
                return false;
            }

            object content = null;
            var deserializationFailed = false;
            Exception deserializationError = null;

            var serializerSettings = new JsonSerializerSettings
            {
                Error = (_, args) =>
                {
                    deserializationFailed = true;
                    deserializationError = args.ErrorContext.Error;
                }
            };

            if (_eventTypes.HasTypeFor(eventType))
            {
                content = JsonConvert.DeserializeObject(source.Content, _eventTypes.GetTypeFor(eventType), serializerSettings);
            }
            else
            {
                content = JsonConvert.DeserializeObject(source.Content, serializerSettings);
            }

            if (deserializationFailed)
            {
                error = new InvalidCommittedEventInformation(nameof(source.Content), deserializationError);
                return false;
            }

            if (source.External)
            {
                if (source.ExternalEventReceived == null)
                {
                    error = new MissingCommittedEventInformation(nameof(source.ExternalEventReceived));
                    return false;
                }

                error = null;
                @event = new CommittedExternalEvent(
                    source.EventLogSequenceNumber,
                    source.Occurred.ToDateTimeOffset(),
                    eventSourceId,
                    executionContext,
                    eventType,
                    content,
                    source.Public,
                    source.ExternalEventLogSequenceNumber,
                    source.ExternalEventReceived.ToDateTimeOffset());
                return true;
            }
            else
            {
                error = null;
                @event = new CommittedEvent(
                    source.EventLogSequenceNumber,
                    source.Occurred.ToDateTimeOffset(),
                    eventSourceId,
                    executionContext,
                    eventType,
                    content,
                    source.Public);
                return true;
            }
        }

        /// <inheritdoc/>
        public CommittedEvent ToSDK(Contracts.CommittedEvent source)
            => TryToSDK(source, out var @event, out var error) ? @event : throw error;

        /// <inheritdoc/>
        public bool TryToSDK(IEnumerable<Contracts.CommittedEvent> source, out CommittedEvents events, out Exception error)
        {
            events = default;

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            var list = new List<CommittedEvent>();
            foreach (var sourceEvent in source)
            {
                if (!TryToSDK(sourceEvent, out var @event, out error))
                {
                    return false;
                }
                else
                {
                    list.Add(@event);
                }
            }

            events = new CommittedEvents(list);
            error = null;
            return true;
        }

        /// <inheritdoc/>
        public CommittedEvents ToSDK(IEnumerable<Contracts.CommittedEvent> source)
            => TryToSDK(source, out var events, out var error) ? events : throw error;

        /// <inheritdoc/>
        public bool TryToSDK(Contracts.CommitEventsResponse source, out CommitEventsResult result, out Exception error)
        {
            result = default;

            if (!TryToSDK(source.Events, out var events, out error))
            {
                return false;
            }

            error = null;
            result = new CommitEventsResult(source.Failure, events);
            return true;
        }

        /// <inheritdoc/>
        public CommitEventsResult ToSDK(Contracts.CommitEventsResponse source)
            => TryToSDK(source, out var result, out var error) ? result : throw error;

        /// <inheritdoc/>
        public bool TryToProtobuf(UncommittedEvent source, out Contracts.UncommittedEvent @event, out Exception error)
        {
            @event = default;

            var content = string.Empty;
            var serializationFailed = false;
            Exception serializationError = null;
            var serializerSettings = new JsonSerializerSettings
            {
                Error = (_, args) =>
                {
                    serializationFailed = true;
                    serializationError = args.ErrorContext.Error;
                },
                Formatting = Formatting.None,
            };
            content = JsonConvert.SerializeObject(source.Content, serializerSettings);

            if (serializationFailed)
            {
                error = new CouldNotSerializeEventContent(source.Content, serializationError);
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

        /// <inheritdoc/>
        public Contracts.UncommittedEvent ToProtobuf(UncommittedEvent source)
            => TryToProtobuf(source, out var @event, out var error) ? @event : throw error;

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
                {
                    return false;
                }
                else
                {
                    list.Add(@event);
                }
            }

            error = null;
            events = list;
            return true;
        }

        /// <inheritdoc/>
        public IEnumerable<Contracts.UncommittedEvent> ToProtobuf(UncommittedEvents source)
            => TryToProtobuf(source, out var events, out var error) ? events : throw error;
    }
}
