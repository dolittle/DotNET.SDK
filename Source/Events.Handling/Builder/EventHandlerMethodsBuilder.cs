// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Artifacts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents a building event handler methods.
    /// </summary>
    public class EventHandlerMethodsBuilder
    {
        readonly IDictionary<Type, IEventHandlerMethod> _handleMethodByType = new Dictionary<Type, IEventHandlerMethod>();
        readonly IDictionary<EventType, IEventHandlerMethod> _handleMethodByArtifact = new Dictionary<EventType, IEventHandlerMethod>();
        readonly EventHandlerId _eventHandlerId;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethodsBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHandlerMethodsBuilder(EventHandlerId eventHandlerId, ILogger logger)
        {
            _eventHandlerId = eventHandlerId;
            _logger = logger;
        }

        /// <summary>
        /// Gets a value indicating whether or not there are invalid event handler methods and the event handler shouldn't be registered.
        /// </summary>
        public bool HasInvalidMethods { get; private set; }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="method">The <see cref="TypedEventHandlerSignature{T}" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle<T>(TypedEventHandlerSignature<T> method)
            where T : class
        {
            if (!_handleMethodByType.TryAdd(typeof(T), new TypedEventHandlerMethod<T>(_eventHandlerId, method)))
            {
                _logger.LogWarning("Event handler {EventHandler} already handles event of type {EventType}", _eventHandlerId, typeof(T));
                HasInvalidMethods = true;
            }

            return this;
        }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="EventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventType eventType, EventHandlerSignature method)
        {
            if (!_handleMethodByArtifact.TryAdd(eventType, new EventHandlerMethod(method)))
            {
                _logger.LogWarning("Event handler {EventHandler} already handles event with event type {EventType}", _eventHandlerId, eventType);
                HasInvalidMethods = true;
            }

            return this;
        }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="method">The <see cref="EventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, EventHandlerSignature method)
            => Handle(new EventType(eventTypeId), method);

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="EventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, Generation eventTypeGeneration, EventHandlerSignature method)
            => Handle(new EventType(eventTypeId, eventTypeGeneration), method);

        /// <summary>
        /// Builds the event handler methods.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <returns>The event handler methods.</returns>
        public IDictionary<EventType, IEventHandlerMethod> Build(IEventTypes eventTypes)
        {
            var eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();
            foreach ((var eventType, var method) in _handleMethodByArtifact)
            {
                eventTypesToMethods.Add(eventType, method);
            }

            foreach ((var type, var method) in _handleMethodByType)
            {
                var eventType = eventTypes.GetFor(type);
                if (!eventTypesToMethods.TryAdd(eventType, method))
                {
                    _logger.LogWarning("Event handler {EventHandler} already handles event with event type {EventType}", _eventHandlerId, eventType);
                    HasInvalidMethods = true;
                }
            }

            return eventTypesToMethods;
        }
    }
}
