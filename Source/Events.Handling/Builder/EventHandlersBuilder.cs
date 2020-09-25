// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents the builder for configuring event handlers.
    /// </summary>
    public class EventHandlersBuilder : ICanBuildAndRegisterAnEventHandler
    {
        readonly IList<ICanBuildAndRegisterAnEventHandler> _builders = new List<ICanBuildAndRegisterAnEventHandler>();
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersBuilder"/> class.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventHandlersBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Start building an event handler.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="callback">Callback for building the event handler.</param>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlersBuilder CreateEventHandler(EventHandlerId eventHandlerId, Action<EventHandlerBuilder> callback)
        {
            var builder = new EventHandlerBuilder(eventHandlerId, _loggerFactory);
            callback(builder);
            _builders.Add(builder);
            return this;
        }

        /// <summary>
        /// Registers a <see cref="Type" /> as an event handler class.
        /// </summary>
        /// <typeparam name="TEventHandler">The <see cref="Type" /> that is the event handler class.</typeparam>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlersBuilder RegisterEventHandler<TEventHandler>()
            where TEventHandler : class
        {
            _builders.Add(new ConventionEventHandlerBuilder(typeof(TEventHandler), _loggerFactory));
            return this;
        }

        /// <summary>
        /// Registers a <see cref="Type" /> as an event handler class.
        /// </summary>
        /// <typeparam name="TEventHandler">The <see cref="Type" /> that is the event handler class.</typeparam>
        /// <param name="eventHandlerInstance">The <typeparamref name="TEventHandler"/>.</param>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlersBuilder RegisterEventHandler<TEventHandler>(TEventHandler eventHandlerInstance)
            where TEventHandler : class
        {
            _builders.Add(new ConventionEventHandlerBuilder(eventHandlerInstance, _loggerFactory));
            return this;
        }

        /// <summary>
        /// Registers a <see cref="Type" /> as an event handler class.
        /// </summary>
        /// <param name="eventHandlerInstance">The event handler instance.</param>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlersBuilder RegisterEventHandler(object eventHandlerInstance)
        {
            _builders.Add(new ConventionEventHandlerBuilder(eventHandlerInstance, _loggerFactory));
            return this;
        }

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IContainer container,
            CancellationToken cancellation)
        {
            foreach (var builder in _builders)
            {
                builder.BuildAndRegister(eventProcessors, eventTypes, processingConverter, container, cancellation);
            }
        }
    }
}