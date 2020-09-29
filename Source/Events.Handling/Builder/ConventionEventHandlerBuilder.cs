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
        public abstract BuildEventHandlerResult BuildAndRegister(
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
        /// <returns>The <see cref="BuildEventHandlerResult"/>.</returns>
        protected BuildEventHandlerResult BuildAndRegister(
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
            if (!TryGetEventHandlerInformation(out var eventHandlerId, out var partitioned, out var scopeId))
            {
                return new BuildEventHandlerResult(
                    null,
                    $"The event handler class {EventHandlerType} needs to be decorated with an [{typeof(EventHandlerAttribute).Name}]");
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
                out var buildResult))
            {
                return buildResult;
            }

            var eventHandler = new EventHandler(eventHandlerId, scopeId, partitioned, eventTypesToMethods);
            var eventHandlerProcessor = new EventHandlerProcessor(
                eventHandler,
                processingConverter,
                loggerFactory.CreateLogger<EventHandlerProcessor>());

            eventProcessors.Register(
                eventHandlerProcessor,
                new EventHandlerProtocol(),
                cancellation);
            return new BuildEventHandlerResult();
        }

        bool TryBuildHandlerMethods(
            EventHandlerId eventHandlerId,
            IEventTypes eventTypes,
            CreateUntypedHandleMethod createUntypedHandlerMethod,
            CreateTypedHandleMethod createTypedHandlerMethod,
            IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
            out BuildEventHandlerResult buildResult)
        {
            var publicMethods = EventHandlerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            var aggregatedWarnings = new List<string>();
            if (!TryAddDecoratedHandlerMethods(
                publicMethods,
                eventHandlerId,
                createUntypedHandlerMethod,
                eventTypesToMethods,
                out var decoratedHandlerMethodsWarnings))
            {
                aggregatedWarnings.AddRange(decoratedHandlerMethodsWarnings.Warnings);
            }

            if (!TryAddConventionHandlerMethods(
                publicMethods,
                eventHandlerId,
                eventTypes,
                createTypedHandlerMethod,
                eventTypesToMethods,
                out var conventionHandlerMethodsWarnings))
            {
                aggregatedWarnings.AddRange(conventionHandlerMethodsWarnings.Warnings);
            }

            if (aggregatedWarnings.Count > 0)
            {
                buildResult = new BuildEventHandlerResult(eventHandlerId, aggregatedWarnings);
                return false;
            }

            if (eventTypesToMethods.Count == 0)
            {
                buildResult = new BuildEventHandlerResult(
                    eventHandlerId,
                    $"There are no event handler methods to register in event handler {EventHandlerType}. An event handler method either needs to be decorated with [{typeof(HandlesAttribute).Name}] or have the name {MethodName}");
                return false;
            }

            buildResult = new BuildEventHandlerResult();
            return true;
        }

        bool TryAddDecoratedHandlerMethods(
            IEnumerable<MethodInfo> methods,
            EventHandlerId eventHandlerId,
            CreateUntypedHandleMethod createUntypedHandlerMethod,
            IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
            out EventHandlerBuildWarnings warnings)
        {
            warnings = default;

            var warningMessages = new List<string>();

            foreach (var method in methods.Where(IsDecoratedHandlerMethod).ToArray())
            {
                var shouldAddHandler = true;
                var eventType = (method.GetCustomAttributes(typeof(HandlesAttribute), true)[0] as HandlesAttribute)?.EventType;
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {EventHandlerType} has no parameters, but is decorated with [{typeof(HandlesAttribute).Name}]. An event handler method should take in as paramters an event and an {typeof(EventContext).Name}");
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, eventHandlerId, out var parameterWarnings))
                {
                    warningMessages.AddRange(parameterWarnings.Warnings);
                    shouldAddHandler = false;
                }

                if (eventParameterType != typeof(object))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {EventHandlerType} should only handle an event of type object");
                    shouldAddHandler = false;
                }

                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, createUntypedHandlerMethod(method)))
                {
                    warningMessages.Add($"Event type {eventType} is already handled in event handler {eventHandlerId}");
                }
            }

            if (warningMessages.Count == 0) return true;

            warnings = new EventHandlerBuildWarnings(eventHandlerId, warningMessages);
            return false;
        }

        bool TryAddConventionHandlerMethods(
            IEnumerable<MethodInfo> methods,
            EventHandlerId eventHandlerId,
            IEventTypes eventTypes,
            CreateTypedHandleMethod createTypedHandlerMethod,
            IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
            out EventHandlerBuildWarnings warnings)
        {
            warnings = default;
            var warningMessages = new List<string>();
            foreach (var method in methods.Where(_ => !IsDecoratedHandlerMethod(_) && _.Name == MethodName).ToArray())
            {
                var shouldAddHandler = true;
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {EventHandlerType} has no parameters. An event handler method should take in as paramters an event and an {typeof(EventContext).Name}");
                    shouldAddHandler = false;
                }

                if (eventParameterType == typeof(object))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {EventHandlerType} cannot handle an untyped event when not decorated with [{typeof(HandlesAttribute).Name}]");
                    shouldAddHandler = false;
                }

                if (!eventTypes.HasFor(eventParameterType))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {EventHandlerType} handles event of type {eventParameterType}, but it is not associated to any event type");
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, eventHandlerId, out var parameterWarnings))
                {
                    warningMessages.AddRange(parameterWarnings.Warnings);
                    shouldAddHandler = false;
                }

                var eventType = eventTypes.GetFor(eventParameterType);
                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, createTypedHandlerMethod(eventParameterType, method)))
                {
                    warningMessages.Add($"Event type {eventParameterType} is already handled in event handler {eventHandlerId}");
                }
            }

            if (warningMessages.Count == 0) return true;

            warnings = new EventHandlerBuildWarnings(eventHandlerId, warningMessages);
            return false;
        }

        bool ParametersAreOkay(MethodInfo method, EventHandlerId eventHandlerId, out EventHandlerBuildWarnings buildWarnings)
        {
            buildWarnings = default;
            var warnings = new List<string>();
            if (!SecondMethodParameterIsEventContext(method))
            {
                warnings.Add($"Event handler method {method} on event handler {EventHandlerType} needs to have two parameters where the second parameter is {typeof(EventContext)}");
            }

            if (!MethodHasNoExtraParameters(method))
            {
                warnings.Add($"Event handler method {method} on event handler {EventHandlerType} needs to only have two parameters where the first is the event to handle and the second is {typeof(EventContext)}");
            }

            if (MethodReturnsAsyncVoid(method) || (!MethodReturnsVoid(method) && !MethodReturnsTask(method)))
            {
                warnings.Add($"Event handler method {method} on event handler {EventHandlerType} needs to return either {typeof(void)} or {typeof(Task)}");
            }

            if (warnings.Count == 0) return true;
            buildWarnings = new EventHandlerBuildWarnings(eventHandlerId, warnings);
            return false;
        }

        bool TryGetEventHandlerInformation(out EventHandlerId eventHandlerId, out bool partitioned, out ScopeId scopeId)
        {
            eventHandlerId = default;
            partitioned = default;
            scopeId = default;
            var eventHandler = EventHandlerType.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() as EventHandlerAttribute;
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