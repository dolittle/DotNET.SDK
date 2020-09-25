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
        EventHandlerMethodsBuilder _methodsBuilder;

        ScopeId _scopeId = ScopeId.Default;

        bool _partitioned = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        public EventHandlerBuilder(EventHandlerId eventHandlerId) => _eventHandlerId = eventHandlerId;

        /// <summary>
        /// Defines the event handler to be partitioned - this is default for an event handler.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerMethodsBuilder Partitioned()
        {
            _partitioned = true;
            _methodsBuilder = new EventHandlerMethodsBuilder(_eventHandlerId);
            return _methodsBuilder;
        }

        /// <summary>
        /// Defines the event handler to be unpartitioned. By default it will be partitioned.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerMethodsBuilder Unpartitioned()
        {
            _partitioned = false;
            _methodsBuilder = new EventHandlerMethodsBuilder(_eventHandlerId);
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
        public BuildEventHandlerResult BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IContainer container,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            if (_methodsBuilder == default)
            {
                return new BuildEventHandlerResult(
                    _eventHandlerId,
                    $"No event handler methods are configured for event handler {_eventHandlerId}");
            }

            var buildResult = _methodsBuilder.TryBuild(eventTypes, out var eventTypesToMethods);
            if (!buildResult.Succeeded) return buildResult;

            var eventHandler = new EventHandler(_eventHandlerId, _scopeId, _partitioned, eventTypesToMethods);
            var eventHandlerProcessor = new EventHandlerProcessor(eventHandler, processingConverter, loggerFactory.CreateLogger<EventHandlerProcessor>());
            eventProcessors.Register(
                eventHandlerProcessor,
                new EventHandlerProtocol(),
                cancellation);

            return new BuildEventHandlerResult();
        }
    }
}
