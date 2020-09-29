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
    public class EventHandlersBuilder
    {
        readonly IList<ICanBuildAndRegisterAnEventHandler> _builders = new List<ICanBuildAndRegisterAnEventHandler>();

        /// <summary>
        /// Start building an event handler.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="callback">Callback for building the event handler.</param>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlersBuilder CreateEventHandler(EventHandlerId eventHandlerId, Action<EventHandlerBuilder> callback)
        {
            var builder = new EventHandlerBuilder(eventHandlerId);
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
            => RegisterEventHandler(typeof(TEventHandler));

        /// <summary>
        /// Registers a <see cref="Type" /> as an event handler class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the event handler.</param>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlersBuilder RegisterEventHandler(Type type)
        {
            _builders.Add(new ConventionTypeEventHandlerBuilder(type));
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
            _builders.Add(new ConventionInstanceEventHandlerBuilder(eventHandlerInstance));
            return this;
        }

        /// <summary>
        /// Build and registers event handlers.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="container">The <see cref="IContainer" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IContainer container,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            var logger = loggerFactory.CreateLogger<EventHandlersBuilder>();
            foreach (var builder in _builders)
            {
                var buildResult = builder.BuildAndRegister(eventProcessors, eventTypes, processingConverter, container, loggerFactory, cancellation);
                if (!buildResult.Succeeded) logger.LogWarning(buildResult.Warnings.ToString());
            }
        }
    }
}