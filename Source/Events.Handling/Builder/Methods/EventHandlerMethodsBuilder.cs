// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Handling.Builder.Methods;

/// <summary>
/// Represents a building event handler methods.
/// </summary>
public class EventHandlerMethodsBuilder : IEventHandlerMethodsBuilder
{
    readonly List<(Type, IEventHandlerMethod)> _handleMethodAndTypes = new();
    readonly List<(EventType, IEventHandlerMethod)> _handleMethodAndArtifacts = new();
    readonly EventHandlerId _eventHandlerId;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerMethodsBuilder"/> class.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
    public EventHandlerMethodsBuilder(EventHandlerId eventHandlerId) => _eventHandlerId = eventHandlerId;


    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle<T>(TaskEventHandlerSignature<T> method)
        where T : class
    {
        _handleMethodAndTypes.Add((typeof(T), new TypedEventHandlerMethod<T>(method)));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle<T>(VoidEventHandlerSignature<T> method)
        where T : class
    {
        _handleMethodAndTypes.Add((typeof(T), new TypedEventHandlerMethod<T>(method)));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle(EventType eventType, TaskEventHandlerSignature method)
    {
        _handleMethodAndArtifacts.Add((eventType, new EventHandlerMethod(method)));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle(EventType eventType, VoidEventHandlerSignature method)
    {
        _handleMethodAndArtifacts.Add((eventType, new EventHandlerMethod(method)));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, TaskEventHandlerSignature method)
        => Handle(new EventType(eventTypeId), method);

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, VoidEventHandlerSignature method)
        => Handle(new EventType(eventTypeId), method);

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, Generation eventTypeGeneration, TaskEventHandlerSignature method)
        => Handle(new EventType(eventTypeId, eventTypeGeneration), method);

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, Generation eventTypeGeneration, VoidEventHandlerSignature method)
        => Handle(new EventType(eventTypeId, eventTypeGeneration), method);

    /// <summary>
    /// Builds the event handler methods.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="eventTypesToMethods">The output <see cref="IDictionary{TKey, TValue}" /> of <see cref="EventType" /> to <see cref="IEventHandlerMethod" /> map.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults" />.</param>
    /// <returns>Whether all the event handler methods could be built.</returns>
    public bool TryAddEventHandlerMethods(IEventTypes eventTypes, IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods, IClientBuildResults buildResults)
    {
        var okay = true;
        foreach (var (eventType, method) in _handleMethodAndArtifacts)
        {
            if (eventTypesToMethods.TryAdd(eventType, method))
            {
                continue;
            }
            okay = false;
            buildResults.AddFailure($"Event handler {_eventHandlerId} already handles event with event type {eventType}");
        }
        foreach (var (type, method) in _handleMethodAndTypes)
        {
            var eventType = eventTypes.GetFor(type);
            if (eventTypesToMethods.TryAdd(eventType, method))
            {
                continue;
            }
            okay = false;
            buildResults.AddFailure($"Event handler {_eventHandlerId} already handles event with event type {eventType}");
        }
        return okay;
    }
}
