// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Methods for building <see cref="IEventHandler"/> instances by convention from an instantiated event handler class.
    /// </summary>
    public class ConventionEventHandlerBuilder : ICanBuildAndRegisterAnEventHandler
    {
        readonly Type _eventHandlerType;
        readonly ILogger _logger;
        readonly ILoggerFactory _loggerFactory;
        object _eventHandlerInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionEventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerInstance">The event handler instance.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public ConventionEventHandlerBuilder(object eventHandlerInstance, ILoggerFactory loggerFactory)
        {
            _eventHandlerInstance = eventHandlerInstance;
            _eventHandlerType = eventHandlerInstance.GetType();
            _logger = loggerFactory.CreateLogger<ConventionEventHandlerBuilder>();
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionEventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerType">The event handler <see cref="Type" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public ConventionEventHandlerBuilder(Type eventHandlerType, ILoggerFactory loggerFactory)
        {
            _eventHandlerType = eventHandlerType;
            _logger = loggerFactory.CreateLogger<ConventionEventHandlerBuilder>();
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IContainer container,
            CancellationToken cancellation)
        {
            _eventHandlerInstance ??= container.Get(_eventHandlerType);
            if (!TryGetEventHandlerInformation(out var eventHandlerId, out var partitioned, out var scopeId))
            {
                _logger.LogWarning("The event handler class {EventHandlerType} needs to be decorated with an [{EventHandlerAttribute}]", _eventHandlerType, typeof(EventHandlerAttribute).Name);
                return;
            }

            var eventHandlerMethodsBuilder = new EventHandlerMethodsBuilder(eventHandlerId, _loggerFactory.CreateLogger<EventHandlerMethodsBuilder>());
            var publicMethods = _eventHandlerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

            AddDecoratedHandlerMethods(
                publicMethods,
                eventTypes,
                eventHandlerMethodsBuilder);
            AddConventionHandlerMethods(
                publicMethods,
                eventTypes,
                eventHandlerMethodsBuilder);

            var eventTypesToMethods = eventHandlerMethodsBuilder.Build(eventTypes);
            if (eventHandlerMethodsBuilder.HasInvalidMethods)
            {
                _logger.LogWarning("Event handler {EventHandler} has invalid event handler methods. Event handler will not be registered", eventHandlerId);
                return;
            }

            if (eventTypesToMethods.Count == 0)
            {
                _logger.LogWarning("There are no event handlers methods to register in event handler {EventHandler}", _eventHandlerType);
                return;
            }

            var eventHandler = new EventHandler(eventHandlerId, scopeId, partitioned, eventTypesToMethods);
            var eventHandlerProcessor = new EventHandlerProcessor(eventHandler, processingConverter, _loggerFactory.CreateLogger<EventHandlerProcessor>());
            eventProcessors.Register(
                eventHandlerProcessor,
                new EventHandlerProtocol(),
                cancellation);
        }

        void AddDecoratedHandlerMethods(IEnumerable<MethodInfo> methods, IEventTypes eventTypes, EventHandlerMethodsBuilder eventHandlerMethodsBuilder)
        {
            foreach (var method in methods.Where(IsDecoratedHandlerMethod))
            {
                var eventType = (method.GetCustomAttributes(typeof(HandlesAttribute), true)[0] as HandlesAttribute).EventType;
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    _logger.LogWarning(
                        "Method {EventHandlerMethod} on event handler {EventHandler} has no parameters, but is decorated with [{HandlesAttribute}]. An event handler method should take in as paramters an event and an {EventContext}",
                        method,
                        _eventHandlerType,
                        typeof(HandlesAttribute).Name);
                    return;
                }

                if (!ParametersAreOkay(method)) return;

                if (eventTypes.HasTypeFor(eventType))
                {
                    var associatedType = eventTypes.GetTypeFor(eventType);
                    if (eventParameterType != typeof(object) && eventParameterType != associatedType)
                    {
                        _logger.LogWarning(
                            "Method {EventHandlerMethod} on event handler {EventHandler} handles {EventType} which is associated with {AssociatedType}, but first parameter is {EventParameterType}. The first parameter should be either object or {AssociatedType}",
                            method,
                            _eventHandlerType,
                            eventType,
                            associatedType,
                            eventParameterType,
                            associatedType);
                        return;
                    }

                    if (eventParameterType == associatedType) AddTypedHandleSignatureToBuilder(eventParameterType, method, eventHandlerMethodsBuilder);
                    else AddUntypedHandleSignatureToBuilder(method, eventType, eventHandlerMethodsBuilder);
                }
                else
                {
                    if (eventParameterType != typeof(object))
                    {
                        _logger.LogWarning(
                            "Method {EventHandlerMethod} on event handler {EventHandler} can only handle an event of type object because there are no type associated to {EventType}",
                            method,
                            _eventHandlerType,
                            eventType);
                        return;
                    }

                    AddUntypedHandleSignatureToBuilder(method, eventType, eventHandlerMethodsBuilder);
                }
            }
        }

        void AddConventionHandlerMethods(IEnumerable<MethodInfo> methods, IEventTypes eventTypes, EventHandlerMethodsBuilder eventHandlerMethodsBuilder)
        {
            foreach (var method in methods.Where(_ => !IsDecoratedHandlerMethod(_)))
            {
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    _logger.LogWarning(
                        "Method {EventHandlerMethod} on event handler {EventHandler} has no parameters. An event handler method should take in as paramters an event and an {EventContext}",
                        method,
                        _eventHandlerType,
                        typeof(EventContext).Name);
                    return;
                }

                if (eventParameterType == typeof(object))
                {
                    _logger.LogWarning(
                        "Method {EventHandlerMethod} on event handler {EventHandler} cannot handle an untyped event when not decorated with [{HandlesAttribute}]",
                        method,
                        _eventHandlerType,
                        typeof(HandlesAttribute).Name);
                    return;
                }

                if (!eventTypes.HasFor(eventParameterType))
                {
                    _logger.LogWarning(
                        "Method {EventHandlerMethod} on event handler {EventHandler} handles event of type {EventParameterType}, but it is not associated to any event type",
                        method,
                        _eventHandlerType,
                        eventParameterType);
                    return;
                }

                if (!ParametersAreOkay(method)) return;

                AddTypedHandleSignatureToBuilder(eventParameterType, method, eventHandlerMethodsBuilder);
            }
        }

        bool ParametersAreOkay(MethodInfo method)
        {
            if (!SecondMethodParameterIsEventContext(method))
            {
                _logger.LogWarning(
                    "Method {EventHandlerMethod} on event handler {EventHandler} needs to have two parameters where the second parameter is {EventContext}",
                    method,
                    _eventHandlerType,
                    typeof(EventContext));
                return false;
            }

            if (!MethodHasNoExtraParameters(method))
            {
                _logger.LogWarning(
                    "Method {EventHandlerMethod} on event handler {EventHandler} needs to only have two parameters where the first is the event to handle and the second is {EventContext}",
                    method,
                    _eventHandlerType,
                    typeof(EventContext));
                return false;
            }

            if (!MethodReturnsVoid(method) && !MethodReturnsTask(method))
            {
                _logger.LogWarning(
                    "Method {EventHandlerMethod} on event handler {EventHandler} needs to return either {Void} or {Task}",
                    method,
                    _eventHandlerType,
                    typeof(void),
                    typeof(Task));
                return false;
            }

            return true;
        }

        void AddUntypedHandleSignatureToBuilder(MethodInfo method, EventType eventType, EventHandlerMethodsBuilder builder)
        {
            var eventHandlerSignatureType = method.ReturnType == typeof(Task) ? typeof(EventHandlerSignature) : typeof(VoidEventHandlerSignature);
            var eventHandlerSignature = Delegate.CreateDelegate(eventHandlerSignatureType, _eventHandlerInstance, method);
            var builderHandleMethod = typeof(EventHandlerMethodsBuilder)
                                        .GetMethod(nameof(EventHandlerMethodsBuilder.Handle), new[] { typeof(EventType), eventHandlerSignatureType });
            builder.Handle(eventType, eventHandlerSignature as EventHandlerSignature);
        }

        void AddTypedHandleSignatureToBuilder(Type eventParameterType, MethodInfo method, EventHandlerMethodsBuilder builder)
        {
            var eventHandlerSignatureGenericTypeDefinition = method.ReturnType == typeof(Task) ?
                                                typeof(TypedEventHandlerSignature<>)
                                                : typeof(VoidTypedEventHandlerSignature<>);
            var eventHandlerSignatureType = eventHandlerSignatureGenericTypeDefinition.MakeGenericType(eventParameterType);
            var eventHandlerSignature = Delegate.CreateDelegate(eventHandlerSignatureType, _eventHandlerInstance, method);
            var builderHandleMethod = typeof(EventHandlerMethodsBuilder)
                                        .GetMethods()
                                        .Where(_ => _.IsGenericMethod && _.Name == nameof(EventHandlerMethodsBuilder.Handle))
                                        .Single(_ => _.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == eventHandlerSignatureGenericTypeDefinition)
                                        .MakeGenericMethod(eventParameterType);
            builderHandleMethod.Invoke(builder, new[] { eventHandlerSignature });
        }

        bool IsDecoratedHandlerMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(HandlesAttribute), true).FirstOrDefault() != default;

        bool TryGetEventHandlerInformation(out EventHandlerId eventHandlerId, out bool partitioned, out ScopeId scopeId)
        {
            eventHandlerId = default;
            partitioned = default;
            scopeId = default;
            var eventHandler = _eventHandlerType.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() as EventHandlerAttribute;
            if (eventHandler == default) return false;

            eventHandlerId = eventHandler.Identifier;
            partitioned = eventHandler.Partitioned;
            scopeId = eventHandler.ScopeId;
            return true;
        }

        bool TryGetFirstMethodParameterType(MethodInfo method, out Type type)
        {
            type = default;
            if (method.GetParameters().Length == 0) return false;

            type = method.GetParameters()[0].ParameterType;
            return true;
        }

        bool SecondMethodParameterIsEventContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EventContext);

        bool MethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;

        bool MethodReturnsTask(MethodInfo method)
            => method.ReturnType == typeof(Task);

        bool MethodReturnsVoid(MethodInfo method)
            => method.ReturnType == typeof(void);
    }
}