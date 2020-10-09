// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Protobuf;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IConvertEventResponsestoSDK"/>.
    /// </summary>
    public class EventResponseToSDKConverter : IConvertEventResponsestoSDK
    {
        readonly ISerializeEventContent _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventResponseToSDKConverter"/> class.
        /// </summary>
        /// <param name="serializer"><see cref="ISerializeEventContent"/> for deserializing event contents.</param>
        public EventResponseToSDKConverter(ISerializeEventContent serializer)
        {
            _serializer = serializer;
        }

        /// <inheritdoc/>
        public bool TryToSDK(CommitEventsResponse source, out CommittedEvents events, out Exception error)
        {
            events = default;

            if (source.Failure != null)
            {
                error = source.Failure.ToException();
                return false;
            }

            return TryToSDK(source.Events, out events, out error);
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

            if (!_serializer.TryToDeserialize(source.Content, eventType, out var content, out var deserializationError))
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

        bool TryToSDK(IEnumerable<Contracts.CommittedEvent> source, out CommittedEvents events, out Exception error)
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
    }
}
