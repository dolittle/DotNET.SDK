// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        readonly List<ICanBuildAndRegisterAnEventHandler> _builders = new List<ICanBuildAndRegisterAnEventHandler>();
        readonly Dictionary<Type, ICanBuildAndRegisterAnEventHandler> _typedBuilder = new Dictionary<Type, ICanBuildAndRegisterAnEventHandler>();

        /// <summary>
        /// Start building an event handler.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlerBuilder CreateEventHandler(EventHandlerId eventHandlerId)
        {
            var builder = new EventHandlerBuilder(eventHandlerId);
            _builders.Add(builder);
            return builder;
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
            _typedBuilder[type] = new ConventionTypeEventHandlerBuilder(type);
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
            _typedBuilder[typeof(TEventHandler)] = new ConventionInstanceEventHandlerBuilder(eventHandlerInstance);
            return this;
        }

        /// <summary>
        /// Registers all event handler classes from an <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to register the event handler classes from.</param>
        /// <returns>The <see cref="EventHandlersBuilder" /> for continuation.</returns>
        public EventHandlersBuilder RegisterAllFrom(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                if (IsEventHandler(type))
                {
                    RegisterEventHandler(type);
                }
            }

            return this;
        }

        /// <summary>
        /// Build and registers event handlers.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="tenantScopedProvidersBuilder">The <see cref="TenantScopedProvidersBuilder"/>.</param>
        /// <param name="tenantScopedProvidersFactory">The <see cref="Func{TResult}"/> for getting <see cref="ITenantScopedProviders" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancelConnectToken">The <see cref="CancellationToken" />.</param>
        /// <param name="stopProcessingToken">The <see cref="CancellationToken" /> for stopping processing.</param>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
            Func<ITenantScopedProviders> tenantScopedProvidersFactory,
            ILoggerFactory loggerFactory,
            CancellationToken cancelConnectToken,
            CancellationToken stopProcessingToken)
        {
            foreach (var builder in _builders.Concat(_typedBuilder.Values))
            {
                builder.BuildAndRegister(eventProcessors, eventTypes, processingConverter, tenantScopedProvidersBuilder, tenantScopedProvidersFactory, loggerFactory, cancelConnectToken, stopProcessingToken);
            }
        }

        static bool IsEventHandler(Type type)
            => type.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() is EventHandlerAttribute;
    }
}
