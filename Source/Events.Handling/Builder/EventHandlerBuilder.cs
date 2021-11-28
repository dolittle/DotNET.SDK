// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
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
        readonly EventHandlerMethodsBuilder _methodsBuilder;

        ScopeId _scopeId = ScopeId.Default;

        EventHandlerAlias _alias;
        bool _hasAlias;
        bool _partitioned = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        public EventHandlerBuilder(EventHandlerId eventHandlerId)
        {
            _eventHandlerId = eventHandlerId;
            _methodsBuilder = new EventHandlerMethodsBuilder(_eventHandlerId);
        }

        /// <summary>
        /// Defines the event handler to be partitioned - this is default for an event handler.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerMethodsBuilder Partitioned()
        {
            _partitioned = true;
            return _methodsBuilder;
        }

        /// <summary>
        /// Defines the event handler to be unpartitioned. By default it will be partitioned.
        /// </summary>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerMethodsBuilder Unpartitioned()
        {
            _partitioned = false;
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

        /// <summary>
        /// Defines the event handler to have a specific <see cref="EventHandlerAlias" />.
        /// </summary>
        /// <param name="alias">The <see cref="EventHandlerAlias" />.</param>
        /// <returns>The builder for continuation.</returns>
        public EventHandlerBuilder WithAlias(EventHandlerAlias alias)
        {
            _alias = alias;
            _hasAlias = true;
            return this;
        }

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            ITenantScopedProviders tenantScopedProviders,
            ILoggerFactory loggerFactory,
            CancellationToken cancelConnectToken,
            CancellationToken stopProcessingToken)
        {
            var eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();
            if (!_methodsBuilder.TryAddEventHandlerMethods(eventTypes, eventTypesToMethods, loggerFactory.CreateLogger<EventHandlerMethodsBuilder>()))
            {
                loggerFactory
                    .CreateLogger<EventHandlerBuilder>()
                    .LogWarning(
                        "Failed to build event handler {EventHandlerId}. One or more event handler methods could not be built",
                        _eventHandlerId);
                return;
            }

            if (eventTypesToMethods.Count < 1)
            {
                loggerFactory
                    .CreateLogger<EventHandlerBuilder>()
                    .LogWarning(
                        "Failed to build event handler {EventHandlerId}. No event handler methods are configured for event handler",
                        _eventHandlerId);
                return;
            }

            var eventHandler = _hasAlias
                ? new EventHandler(_eventHandlerId, _alias, _scopeId, _partitioned, eventTypesToMethods)
                : new EventHandler(_eventHandlerId, _scopeId, _partitioned, eventTypesToMethods);
            var eventHandlerProcessor = new EventHandlerProcessor(eventHandler, processingConverter, loggerFactory.CreateLogger<EventHandlerProcessor>());
            eventProcessors.Register(
                eventHandlerProcessor,
                new EventHandlerProtocol(),
                cancelConnectToken,
                stopProcessingToken);
        }
    }
}
