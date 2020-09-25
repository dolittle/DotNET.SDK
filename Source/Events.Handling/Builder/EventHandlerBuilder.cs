// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents a building event handlers.
    /// </summary>
    public class EventHandlerBuilder : ICanBuildAndRegisterAnEventHandler
    {
        readonly EventHandlerId _eventHandlerId;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;
        EventHandlerMethodsBuilder _methodsBuilder;

        ScopeId _scopeId = ScopeId.Default;

        bool _partitioned = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventHandlerBuilder(EventHandlerId eventHandlerId, ILoggerFactory loggerFactory)
        {
            _eventHandlerId = eventHandlerId;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<EventHandlerBuilder>();
        }

        /// <summary>
        /// Defines the event handler to be partitioned - this is default for an event handler.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerMethodsBuilder Partitioned()
        {
            _partitioned = true;
            _methodsBuilder = new EventHandlerMethodsBuilder(_eventHandlerId, _loggerFactory.CreateLogger<EventHandlerMethodsBuilder>());
            return _methodsBuilder;
        }

        /// <summary>
        /// Defines the event handler to be unpartitioned. By default it will be partitioned.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerMethodsBuilder Unpartitioned()
        {
            _partitioned = false;
            _methodsBuilder = new EventHandlerMethodsBuilder(_eventHandlerId, _loggerFactory.CreateLogger<EventHandlerMethodsBuilder>());
            return _methodsBuilder;
        }

        /// <summary>
        /// Defines the event handler to operate on a specific <see cref="_scopeId" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="_scopeId" />.</param>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerBuilder InScope(ScopeId scopeId)
        {
            _scopeId = scopeId;
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
            if (_methodsBuilder == default)
            {
                _logger.LogWarning("No event handler methods are configured for event handler {EventHandler}", _eventHandlerId);
                return;
            }

            var eventTypesToMethods = _methodsBuilder.Build(eventTypes);
            if (_methodsBuilder.HasInvalidMethods)
            {
                _logger.LogWarning("Event handler {EventHandler} has invalid event handler methods. Event handler will not be registered", _eventHandlerId);
                return;
            }

            var eventHandler = new EventHandler(_eventHandlerId, _scopeId, _partitioned, eventTypesToMethods);
            var eventHandlerProcessor = new EventHandlerProcessor(eventHandler, processingConverter, _loggerFactory.CreateLogger<EventHandlerProcessor>());
            eventProcessors.Register(
                eventHandlerProcessor,
                new EventHandlerProtocol(),
                cancellation);
        }
    }
}
