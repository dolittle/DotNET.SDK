// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Failures;
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

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

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

            if (!source.Type.TryTo<EventType, EventTypeId>(out var eventType, out var eventTypeError))
            {
                error = new InvalidCommittedEventInformation(nameof(source.Type), eventTypeError);
                return false;
            }

            if (string.IsNullOrWhiteSpace(source.Content))
            {
                error = new MissingCommittedEventInformation(nameof(source.Content));
                return false;
            }

            if (!TryDeserializeContent(source.Content, eventType, out var content, out var deserializationError))
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
                    return false;

                list.Add(@event);
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

            if (source.Failure != null)
            {
                result = new CommitEventsResult(source.Failure.ToSDK(), null);
                error = null;
                return true;
            }

            if (!TryToSDK(source.Events, out var events, out error))
                return false;

            result = new CommitEventsResult(null, events);
            return true;
        }

        /// <inheritdoc/>
        public CommitEventsResult ToSDK(Contracts.CommitEventsResponse source)
            => TryToSDK(source, out var result, out var error) ? result : throw error;

        /// <inheritdoc/>
        public bool TryToSDK(Contracts.CommittedAggregateEvents source, out CommittedAggregateEvents events, out Exception error)
        {
            events = default;

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            if (!source.EventSourceId.TryTo<EventSourceId>(out var eventSourceId, out var eventSourceError))
            {
                error = new InvalidCommittedEventInformation(nameof(source.EventSourceId), eventSourceError);
                return false;
            }

            if (!source.AggregateRootId.TryTo<AggregateRootId>(out var aggregateRootId, out var aggregateRootIdError))
            {
                error = new InvalidCommittedEventInformation(nameof(source.AggregateRootId), aggregateRootIdError);
                return false;
            }

            if (!TryToSDK(source.Events, eventSourceId, aggregateRootId, source.AggregateRootVersion, out var committedAggregateEventList, out error))
                return false;

            events = new CommittedAggregateEvents(eventSourceId, aggregateRootId, committedAggregateEventList);
            error = null;
            return true;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// We have to manually calculate and set the AggregateRootVersion for the events as the
        /// CommittedAggregateEvents.AggregateRootVersion is set to the latest version.
        /// </remarks>
        public bool TryToSDK(
            IEnumerable<Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent> source,
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
                var version = aggregateRootVersion + 1u - (ulong)committedEvents.Length + i;
                var sourceEvent = committedEvents[i];

                if (!TryToSDK(sourceEvent, eventSourceId, aggregateRootId, version, out var @event, out error))
                    return false;

                list.Add(@event);
            }

            events = list;
            error = null;
            return true;
        }

        /// <inheritdoc/>
        public CommittedAggregateEvents ToSDK(Contracts.CommittedAggregateEvents source)
            => TryToSDK(source, out var @events, out var error) ? @events : throw error;

        /// <inheritdoc/>
        public bool TryToSDK(Contracts.CommitAggregateEventsResponse source, out CommitEventsForAggregateResult result, out Exception error)
        {
            result = default;

            if (source.Failure != null)
            {
                result = new CommitEventsForAggregateResult(source.Failure.ToSDK(), null);
                error = null;
                return true;
            }

            if (!TryToSDK(source.Events, out var events, out error))
                return false;

            result = new CommitEventsForAggregateResult(null, events);
            return true;
        }

        /// <inheritdoc/>
        public CommitEventsForAggregateResult ToSDK(Contracts.CommitAggregateEventsResponse source)
            => TryToSDK(source, out var result, out var error) ? result : throw error;

        /// <inheritdoc/>
        public bool TryToSDK(Contracts.FetchForAggregateResponse source, out FetchForAggregateResult result, out Exception error)
        {
            result = default;

            if (source.Failure != null)
            {
                result = new FetchForAggregateResult(source.Failure.ToSDK(), null);
                error = null;
                return true;
            }

            if (!TryToSDK(source.Events, out var events, out error))
                return false;

            result = new FetchForAggregateResult(null, events);
            return true;
        }

        /// <inheritdoc/>
        public FetchForAggregateResult ToSDK(Contracts.FetchForAggregateResponse source)
            => TryToSDK(source, out var result, out var error) ? result : throw error;

        /// <inheritdoc/>
        public bool TryToProtobuf(UncommittedEvent source, out Contracts.UncommittedEvent @event, out Exception error)
        {
            @event = default;

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            if (!TrySerializeContent(source.Content, out var content, out var serializationError))
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
                    return false;

                list.Add(@event);
            }

            error = null;
            events = list;
            return true;
        }

        /// <inheritdoc/>
        public IEnumerable<Contracts.UncommittedEvent> ToProtobuf(UncommittedEvents source)
            => TryToProtobuf(source, out var events, out var error) ? events : throw error;

        /// <inheritdoc/>
        public bool TryToProtobuf(UncommittedAggregateEvent source, out Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent @event, out Exception error)
        {
            @event = default;

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            if (!TrySerializeContent(source.Content, out var content, out var serializationError))
            {
                error = new CouldNotSerializeEventContent(source.Content, serializationError);
                return false;
            }

            error = null;
            @event = new Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent
            {
                Artifact = source.EventType.ToProtobuf(),
                Content = content,
                Public = source.IsPublic,
            };
            return true;
        }

        /// <inheritdoc/>
        public Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent ToProtobuf(UncommittedAggregateEvent source)
            => TryToProtobuf(source, out var events, out var error) ? events : throw error;

        /// <inheritdoc/>
        public bool TryToProtobuf(UncommittedAggregateEvents source, out Contracts.UncommittedAggregateEvents events, out Exception error)
        {
            events = default;

            if (source == null)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            events = new Contracts.UncommittedAggregateEvents
            {
                EventSourceId = source.EventSourceId.ToProtobuf(),
                AggregateRootId = source.AggregateRootId.ToProtobuf(),
                ExpectedAggregateRootVersion = source.ExpectedAggregateRootVersion,
            };

            foreach (var sourceEvent in source)
            {
                if (!TryToProtobuf(sourceEvent, out var @event, out error))
                    return false;

                events.Events.Add(@event);
            }

            error = null;
            return true;
        }

        /// <inheritdoc/>
        public Contracts.UncommittedAggregateEvents ToProtobuf(UncommittedAggregateEvents source)
            => TryToProtobuf(source, out var events, out var error) ? events : throw error;

        bool TryToSDK(
            Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent source,
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

            if (!source.Type.TryTo<EventType, EventTypeId>(out var eventType, out var eventTypeError))
            {
                error = new InvalidCommittedEventInformation(nameof(source.Type), eventTypeError);
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

            if (!TryDeserializeContent(source.Content, eventType, out var content, out var deserializationError))
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

        bool TryDeserializeContent(string source, EventType eventType, out object content, out Exception error)
        {
            content = null;
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
                content = JsonConvert.DeserializeObject(source, _eventTypes.GetTypeFor(eventType), serializerSettings);
            }
            else
            {
                content = JsonConvert.DeserializeObject(source, serializerSettings);
            }

            if (deserializationFailed)
            {
                error = deserializationError;
                return false;
            }

            error = null;
            return true;
        }

        bool TrySerializeContent(object source, out string content, out Exception error)
        {
            content = string.Empty;

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

            content = JsonConvert.SerializeObject(source, serializerSettings);

            if (serializationFailed)
            {
                error = new CouldNotSerializeEventContent(source, serializationError);
                return false;
            }

            error = null;
            return true;
        }
    }
}
