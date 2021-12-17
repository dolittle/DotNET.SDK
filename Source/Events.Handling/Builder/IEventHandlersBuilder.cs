// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines the builder for configuring event handlers.
/// </summary>
public interface IEventHandlersBuilder
{
    /// <summary>
    /// Start building an event handler.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
    /// <returns>The <see cref="IEventHandlersBuilder" /> for continuation.</returns>
    public IEventHandlerBuilder CreateEventHandler(EventHandlerId eventHandlerId);

    /// <summary>
    /// Registers a <see cref="Type" /> as an event handler class.
    /// </summary>
    /// <typeparam name="TEventHandler">The <see cref="Type" /> that is the event handler class.</typeparam>
    /// <returns>The <see cref="IEventHandlersBuilder" /> for continuation.</returns>
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>()
        where TEventHandler : class;

    /// <summary>
    /// Registers a <see cref="Type" /> as an event handler class.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> of the event handler.</param>
    /// <returns>The <see cref="IEventHandlersBuilder" /> for continuation.</returns>
    public IEventHandlersBuilder RegisterEventHandler(Type type);

    /// <summary>
    /// Registers a <see cref="Type" /> as an event handler class.
    /// </summary>
    /// <typeparam name="TEventHandler">The <see cref="Type" /> that is the event handler class.</typeparam>
    /// <param name="eventHandlerInstance">The <typeparamref name="TEventHandler"/>.</param>
    /// <returns>The <see cref="IEventHandlersBuilder" /> for continuation.</returns>
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>(TEventHandler eventHandlerInstance)
        where TEventHandler : class;

    /// <summary>
    /// Registers all event handler classes from an <see cref="Assembly" />.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly" /> to register the event handler classes from.</param>
    /// <returns>The <see cref="IEventHandlersBuilder" /> for continuation.</returns>
    public IEventHandlersBuilder RegisterAllFrom(Assembly assembly);
}
