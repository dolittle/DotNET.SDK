// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events.Builders
{
    /// <summary>
    /// Represents a builder for an event.
    /// </summary>
    public class EventBuilder
    {
        readonly Type _typeOfEvent;
        readonly bool _isPublic;
        readonly EventSourceId _eventSourceId;
        readonly object _event;
        EventType _eventType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBuilder"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="isPublic">Whether event is public.</param>
        public EventBuilder(object @event, EventSourceId eventSourceId, bool isPublic)
        {
            _event = @event;
            _typeOfEvent = @event.GetType();
            _isPublic = isPublic;
            _eventSourceId = eventSourceId;
        }

        /// <summary>
        /// Configures the event type.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" />.</param>
        public void WithEventType(EventType eventType)
        {
            _eventType = eventType;
        }

        /// <summary>
        /// Configures the event type.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" />.</param>
        public void WithEventType(EventTypeId eventTypeId)
            => WithEventType(new EventType(eventTypeId));

        /// <summary>
        /// Configures the event type.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" />.</param>
        /// <param name="generation">The <see cref="Generation" />.</param>
        public void WithEventType(EventTypeId eventTypeId, Generation generation)
            => WithEventType(new EventType(eventTypeId, generation));

        /// <summary>
        /// Builds the information needed to create an uncommitted event.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <returns>The information needed to create an uncommitted event.</returns>
        public (object content, EventType eventType, EventSourceId eventSourceId, bool isPublic) Build(IEventTypes eventTypes)
        {
            var eventType = _eventType;
            if (eventTypes.HasFor(_typeOfEvent))
            {
                var associatedEventType = eventTypes.GetFor(_typeOfEvent);
                ThrowIfEventTypeMismatch(eventType, associatedEventType);
                eventType = associatedEventType;
            }

            return (_event, eventType, _eventSourceId, _isPublic);
        }

        void ThrowIfEventTypeMismatch(EventType eventType, EventType associatedEventType)
        {
            if (eventType != default && eventType != associatedEventType)
                throw new ConfiguredEventTypeDoesNotMatchAssociatedEventType(eventType, associatedEventType);
        }
    }
}