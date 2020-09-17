// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
        public Contracts.UncommittedEvent ToProtobuf(UncommittedEvent @event)
            => new Contracts.UncommittedEvent
            {
                Artifact = @event.EventType.ToProtobuf(),
                EventSourceId = @event.EventSource.ToProtobuf(),
                Public = @event.IsPublic,
                Content = JsonConvert.SerializeObject(@event.Content),
            };

        /// <inheritdoc/>
        public IEnumerable<Contracts.UncommittedEvent> ToProtobuf(UncommittedEvents events)
            => events.Select(ToProtobuf);

        /// <inheritdoc/>
        public CommittedEvent ToSDK(Contracts.CommittedEvent source)
        {
            var eventType = source.Type.To<EventType>();
            var clrEventType = _eventTypes.GetTypeFor(eventType);
            try
            {
                var content = JsonConvert.DeserializeObject(source.Content, clrEventType);
                if (source.External)
                {
                    return new CommittedExternalEvent(
                        source.EventLogSequenceNumber,
                        source.Occurred.ToDateTimeOffset(),
                        source.EventSourceId.To<EventSourceId>(),
                        source.ExecutionContext.ToExecutionContext(),
                        eventType,
                        content,
                        source.Public,
                        source.ExternalEventLogSequenceNumber,
                        source.ExternalEventReceived.ToDateTimeOffset());
                }
                else
                {
                    return new CommittedEvent(
                        source.EventLogSequenceNumber,
                        source.Occurred.ToDateTimeOffset(),
                        source.EventSourceId.To<EventSourceId>(),
                        source.ExecutionContext.ToExecutionContext(),
                        eventType,
                        content,
                        source.Public);
                }
            }
            catch (Exception ex)
            {
                throw new CouldNotDeserializeEvent(
                    source.Type.Id.To<EventTypeId>(),
                    clrEventType,
                    source.Content,
                    source.EventLogSequenceNumber,
                    ex);
            }
        }

        /// <inheritdoc/>
        public CommittedEvents ToSDK(IEnumerable<Contracts.CommittedEvent> source)
            => new CommittedEvents(source.Select(ToSDK).ToList());

        /// <inheritdoc/>
        public CommitEventsResult ToSDK(Contracts.CommitEventsResponse source)
            => new CommitEventsResult(source.Failure, ToSDK(source.Events));
    }
}
