// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Artifacts;

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
        readonly IList<string> _warnings = new List<string>();

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
            if (!_handleMethodByType.TryAdd(typeof(T), new TypedEventHandlerMethod<T>(method)))
            {
                _warnings.Add($"Event handler {_eventHandlerId} already handles event of type {typeof(T)}");
            }

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
            if (!_handleMethodByType.TryAdd(typeof(T), new TypedEventHandlerMethod<T>(method)))
            {
                _warnings.Add($"Event handler {_eventHandlerId} already handles event of type {typeof(T)}");
            }

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
            if (!_handleMethodByArtifact.TryAdd(eventType, new EventHandlerMethod(method)))
            {
                _warnings.Add($"Event handler {_eventHandlerId} already handles event with event type {eventType}");
            }

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
            if (!_handleMethodByArtifact.TryAdd(eventType, new EventHandlerMethod(method)))
            {
                _warnings.Add($"Event handler {_eventHandlerId} already handles event with event type {eventType}");
            }

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
        /// <returns>The event handler methods.</returns>
        public BuildEventHandlerResult TryBuild(IEventTypes eventTypes, out IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods)
        {
            eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();
            foreach ((var eventType, var method) in _handleMethodByArtifact)
            {
                eventTypesToMethods.Add(eventType, method);
            }

            foreach ((var type, var method) in _handleMethodByType)
            {
                var eventType = eventTypes.GetFor(type);
                if (!eventTypesToMethods.TryAdd(eventType, method))
                {
                    _warnings.Add($"Event handler {_eventHandlerId} already handles event with event type {eventType}");
                }
            }

            var result = new BuildEventHandlerResult(_eventHandlerId, _warnings);
            if (!result.Succeeded) eventTypesToMethods = null;
            return result;
        }
    }
}
