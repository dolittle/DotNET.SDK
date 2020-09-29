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
        readonly IList<(Type, IEventHandlerMethod)> _handleMethodAndTypes = new List<(Type, IEventHandlerMethod)>();
        readonly IList<(EventType, IEventHandlerMethod)> _handleMethodAndArtifacts = new List<(EventType, IEventHandlerMethod)>();
        readonly EventHandlerId _eventHandlerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethodsBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        public EventHandlerMethodsBuilder(EventHandlerId eventHandlerId) => _eventHandlerId = eventHandlerId;

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="method">The <see cref="TaskEventHandlerSignature{T}" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle<T>(TaskEventHandlerSignature<T> method)
            where T : class
        {
            _handleMethodAndTypes.Add((typeof(T), new TypedEventHandlerMethod<T>(method)));
            return this;
        }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="method">The <see cref="VoidEventHandlerSignature{T}" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle<T>(VoidEventHandlerSignature<T> method)
            where T : class
        {
            _handleMethodAndTypes.Add((typeof(T), new TypedEventHandlerMethod<T>(method)));
            return this;
        }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="TaskEventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventType eventType, TaskEventHandlerSignature method)
        {
            _handleMethodAndArtifacts.Add((eventType, new EventHandlerMethod(method)));
            return this;
        }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="VoidEventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventType eventType, VoidEventHandlerSignature method)
        {
            _handleMethodAndArtifacts.Add((eventType, new EventHandlerMethod(method)));
            return this;
        }

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="method">The <see cref="TaskEventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, TaskEventHandlerSignature method)
            => Handle(new EventType(eventTypeId), method);

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="method">The <see cref="VoidEventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, VoidEventHandlerSignature method)
            => Handle(new EventType(eventTypeId), method);

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="TaskEventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, Generation eventTypeGeneration, TaskEventHandlerSignature method)
            => Handle(new EventType(eventTypeId, eventTypeGeneration), method);

        /// <summary>
        /// Add a handler method for handling the event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="VoidEventHandlerSignature" />.</param>
        /// <returns>The <see cref="EventHandlerMethodsBuilder" /> for continuation.</returns>
        public EventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, Generation eventTypeGeneration, VoidEventHandlerSignature method)
            => Handle(new EventType(eventTypeId, eventTypeGeneration), method);

        /// <summary>
        /// Builds the event handler methods.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventTypesToMethods">The output <see cref="IDictionary{TKey, TValue}" /> of <see cref="EventType" /> to <see cref="IEventHandlerMethod" /> map.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        /// <returns>Whether all the event handler methods could be built.</returns>
        public bool TryAddEventHandlerMethods(IEventTypes eventTypes, IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods, ILogger logger)
        {
            var okay = true;
            foreach ((var eventType, var method) in _handleMethodAndArtifacts)
            {
                if (!eventTypesToMethods.TryAdd(eventType, method))
                {
                    okay = false;
                    logger.LogWarning(
                        "Event handler {EventHandlerId} already handles event with event type {EventType}",
                        _eventHandlerId,
                        eventType);
                }
            }

            foreach ((var type, var method) in _handleMethodAndTypes)
            {
                var eventType = eventTypes.GetFor(type);
                if (!eventTypesToMethods.TryAdd(eventType, method))
                {
                    okay = false;
                    logger.LogWarning(
                        "Event handler {EventHandlerId} already handles event with event type {EventType}",
                        _eventHandlerId,
                        eventType);
                }
            }

            return okay;
        }
    }
}
