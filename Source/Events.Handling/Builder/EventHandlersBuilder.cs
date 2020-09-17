// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents the builder for configuring event handlers.
    /// </summary>
    public class EventHandlersBuilder
    {
        readonly IList<EventHandlerBuilder> _builders = new List<EventHandlerBuilder>();
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
        /// Builds and registers all the event handlers.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        public void BuildAndRegister(IEventProcessors eventProcessors, IEventTypes eventTypes, IEventProcessingRequestConverter processingRequestConverter, CancellationToken cancellation)
        {
            foreach (var builder in _builders)
            {
                builder.BuildAndRegister(eventProcessors, eventTypes, processingRequestConverter, cancellation);
            }
        }
    }
}