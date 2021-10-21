// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    public abstract class ConventionEventHandlerBuilder : ICanBuildAndRegisterAnEventHandler
    {
        const string MethodName = "Handle";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionEventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerType">The event handler <see cref="Type" />.</param>
        protected ConventionEventHandlerBuilder(Type eventHandlerType) => EventHandlerType = eventHandlerType;

        /// <summary>
        /// Gets the <see cref="Type" /> of the event handler.
        /// </summary>
        protected Type EventHandlerType { get; }

        /// <inheritdoc/>
        public abstract void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IContainer container,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation);

        /// <summary>
        /// Builds and registers event handler.
        /// </summary>
        /// <param name="eventProcessors">THe <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="createUntypedHandlerMethod">The <see cref="CreateUntypedHandleMethod" /> callback.</param>
        /// <param name="createTypedHandlerMethod">The <see cref="CreateTypedHandleMethod" /> callback.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        protected void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            CreateUntypedHandleMethod createUntypedHandlerMethod,
            CreateTypedHandleMethod createTypedHandlerMethod,
            ILoggerFactory loggerFactory,
            ILogger logger,
            CancellationToken cancellation)
        {
            logger.LogDebug("Building event handler from type {EventHandler}", EventHandlerType);
            if (!TryGetEventHandlerInformation(out var eventHandlerId, out var partitioned, out var scopeId, out var alias, out var hasAlias))
            {
                logger.LogWarning("The event handler class {EventHandlerType} needs to be decorated with an [{EventHandlerAttribute}]", EventHandlerType, typeof(EventHandlerAttribute).Name);
            }

            logger.LogTrace(
                "Building {PartitionedOrUnpartitioned} event handler {EventHandlerId} processing events in scope {ScopeId} from type {EventHandler}",
                partitioned ? "partitioned" : "unpartitioned",
                eventHandlerId,
                scopeId,
                EventHandlerType);

            var eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();

            if (!TryBuildHandlerMethods(
                eventHandlerId,
                eventTypes,
                createUntypedHandlerMethod,
                createTypedHandlerMethod,
                eventTypesToMethods,
                logger))
            {
                return;
            }

            var eventHandler = hasAlias 
                ? new EventHandler(eventHandlerId, alias, scopeId, partitioned, eventTypesToMethods)
                : new EventHandler(eventHandlerId, nameof(EventHandlerType), scopeId, partitioned, eventTypesToMethods);
            var eventHandlerProcessor = new EventHandlerProcessor(
                eventHandler,
                processingConverter,
                loggerFactory.CreateLogger<EventHandlerProcessor>());

            eventProcessors.Register(
                eventHandlerProcessor,
                new EventHandlerProtocol(),
                cancellation);
        }

        bool TryBuildHandlerMethods(
            EventHandlerId eventHandlerId,
            IEventTypes eventTypes,
            CreateUntypedHandleMethod createUntypedHandlerMethod,
            CreateTypedHandleMethod createTypedHandlerMethod,
            IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
            ILogger logger)
        {
            var publicMethods = EventHandlerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            var hasWrongMethods = false;
            if (!TryAddDecoratedHandlerMethods(
                publicMethods,
                eventHandlerId,
                createUntypedHandlerMethod,
                eventTypesToMethods,
                logger))
            {
                hasWrongMethods = true;
            }

            if (!TryAddConventionHandlerMethods(
                publicMethods,
                eventHandlerId,
                eventTypes,
                createTypedHandlerMethod,
                eventTypesToMethods,
                logger))
            {
                hasWrongMethods = true;
            }

            if (hasWrongMethods)
            {
                return false;
            }

            if (eventTypesToMethods.Count == 0)
            {
                logger.LogWarning(
                    "There are no event handler methods to register in event handler {EventHandlerType}. An event handler method either needs to be decorated with [{HandlesAttribute}] or have the name {MethodName}",
                    EventHandlerType,
                    typeof(HandlesAttribute).Name,
                    MethodName);
                return false;
            }

            return true;
        }

        bool TryAddDecoratedHandlerMethods(
            IEnumerable<MethodInfo> methods,
            EventHandlerId eventHandlerId,
            CreateUntypedHandleMethod createUntypedHandlerMethod,
            IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
            ILogger logger)
        {
            var allMethodsAdded = true;
            foreach (var method in methods.Where(IsDecoratedHandlerMethod))
            {
                var shouldAddHandler = true;
                var eventType = (method.GetCustomAttributes(typeof(HandlesAttribute), true)[0] as HandlesAttribute)?.EventType;
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    logger.LogWarning(
                        "Event handler method {Method} on event handler {EventHandlerType} has no parameters, but is decorated with [{HandlesAttribute}]. An event handler method should take in as paramters an event and an {EventContext}",
                        method,
                        EventHandlerType,
                        typeof(HandlesAttribute).Name,
                        typeof(EventContext).Name);
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, logger)) shouldAddHandler = false;

                if (eventParameterType != typeof(object))
                {
                    logger.LogWarning(
                        "Event handler method {Method} on event handler {EventHandlerType} should only handle an event of type object",
                        method,
                        EventHandlerType);
                    shouldAddHandler = false;
                }

                if (!method.IsPublic)
                {
                    logger.LogWarning(
                        "Method {Method} on event handler {EventHandlerType} has the signature of an event handler method, but is not public. Event handler methods needs to be public",
                        method,
                        EventHandlerType);
                    shouldAddHandler = false;
                }

                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, createUntypedHandlerMethod(method)))
                {
                    allMethodsAdded = false;
                    logger.LogWarning(
                        "Event type {EventType} is already handled in event handler {EventHandlerId}",
                        eventType,
                        eventHandlerId);
                }

                if (!shouldAddHandler) allMethodsAdded = false;
            }

            return allMethodsAdded;
        }

        bool TryAddConventionHandlerMethods(
            IEnumerable<MethodInfo> methods,
            EventHandlerId eventHandlerId,
            IEventTypes eventTypes,
            CreateTypedHandleMethod createTypedHandlerMethod,
            IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
            ILogger logger)
        {
            var allMethodsAdded = true;
            foreach (var method in methods.Where(_ => !IsDecoratedHandlerMethod(_) && _.Name == MethodName))
            {
                var shouldAddHandler = true;
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    logger.LogWarning(
                        "Event handler method {Method} on event handler {EventHandlerType} has no parameters. An event handler method should take in as paramters an event and an {EventContext}",
                        method,
                        EventHandlerType,
                        typeof(EventContext).Name);
                    shouldAddHandler = false;
                }

                if (eventParameterType == typeof(object))
                {
                    logger.LogWarning(
                        "Event handler method {Method} on event handler {EventHandlerType} cannot handle an untyped event when not decorated with [{HandlesAttribute}]",
                        method,
                        EventHandlerType,
                        typeof(HandlesAttribute).Name);
                    shouldAddHandler = false;
                }

                if (!eventTypes.HasFor(eventParameterType))
                {
                    logger.LogWarning(
                        "Event handler method {Method} on event handler {EventHandlerType} handles event of type {EventParameterType}, but it is not associated to any event type",
                        method,
                        EventHandlerType,
                        eventParameterType);
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, logger)) shouldAddHandler = false;

                if (!method.IsPublic)
                {
                    logger.LogWarning(
                        "Method {Method} on event handler {EventHandlerType} has the signature of an event handler method, but is not public. Event handler methods needs to be public",
                        method,
                        EventHandlerType);
                    shouldAddHandler = false;
                }

                if (!shouldAddHandler) continue;

                var eventType = eventTypes.GetFor(eventParameterType);
                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, createTypedHandlerMethod(eventParameterType, method)))
                {
                    allMethodsAdded = false;
                    logger.LogWarning(
                        "Event type {EventParameterType} is already handled in event handler {EventHandlerId}",
                        eventParameterType,
                        eventHandlerId);
                }

                if (!shouldAddHandler) allMethodsAdded = false;
            }

            return allMethodsAdded;
        }

        bool ParametersAreOkay(MethodInfo method, ILogger logger)
        {
            var okay = true;
            if (!SecondMethodParameterIsEventContext(method))
            {
                okay = false;
                logger.LogWarning(
                    "Event handler method {Method} on event handler {EventHandlerType} needs to have two parameters where the second parameter is {EventContext}",
                    method,
                    EventHandlerType,
                    typeof(EventContext));
            }

            if (!MethodHasNoExtraParameters(method))
            {
                okay = false;
                logger.LogWarning(
                    "Event handler method {Method} on event handler {EventHandlerType} needs to only have two parameters where the first is the event to handle and the second is {EventContext}",
                    method,
                    EventHandlerType,
                    typeof(EventContext));
            }

            if (MethodReturnsAsyncVoid(method) || (!MethodReturnsVoid(method) && !MethodReturnsTask(method)))
            {
                okay = false;
                logger.LogWarning(
                    "Event handler method {Method} on event handler {EventHandlerType} needs to return either {Void} or {Task}",
                    method,
                    EventHandlerType,
                    typeof(void),
                    typeof(Task));
            }

            return okay;
        }

        bool TryGetEventHandlerInformation(out EventHandlerId eventHandlerId, out bool partitioned, out ScopeId scopeId, out EventHandlerAlias alias, out bool hasAlias)
        {
            eventHandlerId = default;
            partitioned = default;
            scopeId = default;
            alias = default;
            hasAlias = default;
            var eventHandler = EventHandlerType.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() as EventHandlerAttribute;
            if (eventHandler == default)
            {
                return false;
            }

            eventHandlerId = eventHandler.Identifier;
            partitioned = eventHandler.Partitioned;
            scopeId = eventHandler.Scope;
            alias = eventHandler.Alias;
            hasAlias = eventHandler.HasAlias;
            return true;
        }

        bool TryGetFirstMethodParameterType(MethodInfo method, out Type type)
        {
            type = default;
            if (method.GetParameters().Length == 0) return false;

            type = method.GetParameters()[0].ParameterType;
            return true;
        }

        bool IsDecoratedHandlerMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(HandlesAttribute), true).FirstOrDefault() != default;

        bool SecondMethodParameterIsEventContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EventContext);

        bool MethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;

        bool MethodReturnsTask(MethodInfo method)
            => method.ReturnType == typeof(Task);

        bool MethodReturnsVoid(MethodInfo method)
            => method.ReturnType == typeof(void);

        bool MethodReturnsAsyncVoid(MethodInfo method)
        {
            var asyncAttribute = typeof(AsyncStateMachineAttribute);
            var isAsyncMethod = (AsyncStateMachineAttribute)method.GetCustomAttribute(asyncAttribute) != null;
            return isAsyncMethod && MethodReturnsVoid(method);
        }
    }
}