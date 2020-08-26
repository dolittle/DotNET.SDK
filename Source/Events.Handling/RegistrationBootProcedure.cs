// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Booting;
using Dolittle.DependencyInversion;
using Dolittle.Events.Handling.EventHorizon;
using Dolittle.Events.Handling.Internal;
using Dolittle.Logging;
using Dolittle.Reflection;
using Dolittle.Types;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Represents an <see cref="ICanPerformBootProcedure"/> that registers event handlers with the Runtime.
    /// </summary>
    public class RegistrationBootProcedure : ICanPerformBootProcedure
    {
        readonly IContainer _container;
        readonly IRegisterEventHandlers _manager;
        readonly IInstancesOf<ICanProvideEventHandlers> _handlerProviders;
        readonly IInstancesOf<ICanProvideExternalEventHandlers> _externalHandlerProviders;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationBootProcedure"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> that will be used to get <see cref="FactoryFor{T}"/> to instantiate event handlers.</param>
        /// <param name="manager">The <see cref="IRegisterEventHandlers"/> that will be used to register the event handlers.</param>
        /// <param name="handlerProviders">Providers of <see cref="ICanHandleEvents"/>.</param>
        /// <param name="externalHandlerProviders">Providers of <see cref="ICanHandleExternalEvents"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public RegistrationBootProcedure(
            IContainer container,
            IRegisterEventHandlers manager,
            IInstancesOf<ICanProvideEventHandlers> handlerProviders,
            IInstancesOf<ICanProvideExternalEventHandlers> externalHandlerProviders,
            ILogger logger)
        {
            _container = container;
            _manager = manager;
            _handlerProviders = handlerProviders;
            _externalHandlerProviders = externalHandlerProviders;
            _logger = logger;
        }

        /// <inheritdoc/>
        public bool CanPerform() => Microservice.Configuration.BootProcedure.HasPerformed && Artifacts.Configuration.BootProcedure.HasPerformed;

        /// <inheritdoc/>
        public void Perform()
        {
            _logger.Debug("Discovering event handlers in boot procedure");
            foreach (var provider in _handlerProviders) RegisterHandlersFromProvider(provider);
            foreach (var provider in _externalHandlerProviders) RegisterHandlersFromProvider(provider);
        }

        void RegisterHandlersFromProvider<THandlerType, TEventType>(ICanProvideHandlers<THandlerType, TEventType> provider)
            where THandlerType : class, ICanHandle<TEventType>
            where TEventType : IEvent
        {
            var type = provider.GetType();
            _logger.Trace("Registering event handlers from {HandlerProvider}", type);
            try
            {
                foreach (var handler in provider.Provide()) RegisterHandler<TEventType>(handler);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error while providing event handlers from {HandlerProvider}", type);
            }
        }

        void RegisterHandler<TEventType>(Type type)
            where TEventType : IEvent
        {
            _logger.Trace("Registering event handler {Handler}", type);

            try
            {
                if (!type.HasAttribute<EventHandlerAttribute>())
                {
                    _logger.Warning("Event handler {Handler} is missing the required [EventHandler(...)] attribute. It will not be registered.");
                    return;
                }

                var handlerId = type.GetCustomAttribute<EventHandlerAttribute>().Id;
                var scopeId = type.HasAttribute<ScopeAttribute>() ? type.GetCustomAttribute<ScopeAttribute>().Id : ScopeId.Default;
                var partitioned = !type.HasAttribute<NotPartitionedAttribute>();

                var factory = _container.Get(typeof(FactoryFor<>).MakeGenericType(type));
                var buildMethod = typeof(ConventionEventHandlerBuilder<TEventType>).GetMethod(nameof(ConventionEventHandlerBuilder<TEventType>.BuildFor));
                var dynamicMethod = buildMethod.MakeGenericMethod(type);

                var handler = (IEventHandler<TEventType>)dynamicMethod.Invoke(null, new[] { factory });
                _manager.Register(handlerId, scopeId, partitioned, handler);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                _logger.Warning(ex, "Error while registering event handler {Handler}", type);
            }
        }
    }
}