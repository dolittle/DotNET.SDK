// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events.Handling.Builder.Methods;

/// <summary>
/// Defines a builder for building event handler methods for an event handler.
/// </summary>
public interface IEventHandlerMethodsBuilder
{
    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="method">The <see cref="TaskEventHandlerSignature{T}" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle<T>(TaskEventHandlerSignature<T> method)
        where T : class;

    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="method">The <see cref="VoidEventHandlerSignature{T}" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle<T>(VoidEventHandlerSignature<T> method)
        where T : class;

    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="TaskEventHandlerSignature" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle(EventType eventType, TaskEventHandlerSignature method);
    
    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="VoidEventHandlerSignature" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle(EventType eventType, VoidEventHandlerSignature method);

    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="method">The <see cref="TaskEventHandlerSignature" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, TaskEventHandlerSignature method);

    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="method">The <see cref="VoidEventHandlerSignature" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, VoidEventHandlerSignature method);

    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="TaskEventHandlerSignature" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, Generation eventTypeGeneration, TaskEventHandlerSignature method);

    /// <summary>
    /// Add a handler method for handling the event.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
    /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
    /// <param name="method">The <see cref="VoidEventHandlerSignature" />.</param>
    /// <returns>The <see cref="IEventHandlerMethodsBuilder" /> for continuation.</returns>
    public IEventHandlerMethodsBuilder Handle(EventTypeId eventTypeId, Generation eventTypeGeneration, VoidEventHandlerSignature method);

}
