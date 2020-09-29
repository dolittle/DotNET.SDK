// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents the signature for an event handler.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task TaskEventHandlerSignature(object @event, EventContext eventContext);

    /// <summary>
    /// Represents the signature for an event handler.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event to handle.</typeparam>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task TaskEventHandlerSignature<T>(T @event, EventContext eventContext)
        where T : class;

     /// <summary>
    /// Represents the signature for an event handler.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate void VoidEventHandlerSignature(object @event, EventContext eventContext);

    /// <summary>
    /// Represents the signature for an event handler.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event to handle.</typeparam>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate void VoidEventHandlerSignature<T>(T @event, EventContext eventContext)
        where T : class;

    /// <summary>
    /// Represents the signature for an event handler method.
    /// </summary>
    /// <param name="instance">The instance of the event handler to invoke the method on.</param>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task TaskEventHandlerMethodSignature(object instance, object @event, EventContext eventContext);

    /// <summary>
    /// Represents the signature for an event handler method.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="instance">The instance of the event handler to invoke the method on.</param>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task TaskEventHandlerMethodSignature<TEvent>(object instance, TEvent @event, EventContext eventContext)
        where TEvent : class;

    /// <summary>
    /// Represents the signature for an event handler method.
    /// </summary>
    /// <param name="instance">The instance of the event handler to invoke the method on.</param>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate void VoidEventHandlerMethodSignature(object instance, object @event, EventContext eventContext);

    /// <summary>
    /// Represents the signature for an event handler method.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="instance">The instance of the event handler to invoke the method on.</param>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate void VoidEventHandlerMethodSignature<TEvent>(object instance, TEvent @event, EventContext eventContext)
        where TEvent : class;
}