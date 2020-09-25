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
        const string MethodName = "Handle";
        readonly Type _eventHandlerType;
        object _eventHandlerInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionEventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerInstance">The event handler instance.</param>
        public ConventionEventHandlerBuilder(object eventHandlerInstance)
        {
            _eventHandlerInstance = eventHandlerInstance;
            _eventHandlerType = eventHandlerInstance.GetType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionEventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerType">The event handler <see cref="Type" />.</param>
        public ConventionEventHandlerBuilder(Type eventHandlerType) => _eventHandlerType = eventHandlerType;

        /// <inheritdoc/>
        public BuildEventHandlerResult BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IContainer container,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            _eventHandlerInstance ??= container.Get(_eventHandlerType);
            var logger = loggerFactory.CreateLogger<ConventionEventHandlerBuilder>();
            logger.LogTrace("Building event handler from type {EventHandler}", _eventHandlerType);
            if (!TryGetEventHandlerInformation(out var eventHandlerId, out var partitioned, out var scopeId))
            {
                return new BuildEventHandlerResult(
                    null,
                    $"The event handler class {_eventHandlerType} needs to be decorated with an [{typeof(EventHandlerAttribute).Name}]");
            }

            logger.LogTrace(
                "Building {PartitionedOrUnpartitioned} event handler {EventHandlerId} processing events in scope {ScopeId} from type {EventHandler}",
                partitioned ? "partitioned" : "unpartitioned",
                eventHandlerId,
                scopeId,
                _eventHandlerType);

            if (!TryBuildHandlerMethods(
                eventHandlerId,
                eventTypes,
                out var eventTypesToMethods,
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
            out IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
            out BuildEventHandlerResult buildResult)
        {
            var eventHandlerMethodsBuilder = new EventHandlerMethodsBuilder(eventHandlerId);
            var publicMethods = _eventHandlerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

            var aggregatedWarnings = new List<string>();
            if (!TryAddDecoratedHandlerMethods(
                publicMethods,
                eventHandlerId,
                eventHandlerMethodsBuilder,
                out var decoratedHandlerMethodsWarnings))
            {
                aggregatedWarnings.AddRange(decoratedHandlerMethodsWarnings.Warnings);
            }

            if (!TryAddConventionHandlerMethods(
                publicMethods,
                eventHandlerId,
                eventTypes,
                eventHandlerMethodsBuilder,
                out var conventionHandlerMethodsWarnings))
            {
                aggregatedWarnings.AddRange(conventionHandlerMethodsWarnings.Warnings);
            }

            var eventHandlerMethodsBuildResult = eventHandlerMethodsBuilder.TryBuild(eventTypes, out eventTypesToMethods);

            if (!eventHandlerMethodsBuildResult.Succeeded) aggregatedWarnings.AddRange(eventHandlerMethodsBuildResult.Warnings.Warnings);

            if (aggregatedWarnings.Count > 0)
            {
                buildResult = new BuildEventHandlerResult(eventHandlerId, aggregatedWarnings);
                return false;
            }

            if (eventTypesToMethods.Count == 0)
            {
                buildResult = new BuildEventHandlerResult(
                    eventHandlerId,
                    $"There are no event handler methods to register in event handler {_eventHandlerType}. An event handler method either needs to be decorated with [{typeof(HandlesAttribute).Name}] or have the name {MethodName}");
                return false;
            }

            buildResult = new BuildEventHandlerResult();
            return true;
        }

        bool TryAddDecoratedHandlerMethods(
            IEnumerable<MethodInfo> methods,
            EventHandlerId eventHandlerId,
            EventHandlerMethodsBuilder eventHandlerMethodsBuilder,
            out EventHandlerBuildWarnings warnings)
        {
            warnings = default;
            var warningMessages = new List<string>();
            foreach (var method in methods.Where(IsDecoratedHandlerMethod))
            {
                var shouldAddHandler = true;
                var eventType = (method.GetCustomAttributes(typeof(HandlesAttribute), true)[0] as HandlesAttribute)?.EventType;
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {_eventHandlerType} has no parameters, but is decorated with [{typeof(HandlesAttribute).Name}]. An event handler method should take in as paramters an event and an {typeof(EventContext).Name}");
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, eventHandlerId, out var parameterWarnings))
                {
                    warningMessages.AddRange(parameterWarnings.Warnings);
                    shouldAddHandler = false;
                }

                if (eventParameterType != typeof(object))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {_eventHandlerType} should only handle an event of type object");
                    shouldAddHandler = false;
                }

                if (shouldAddHandler) AddUntypedHandleSignatureToBuilder(method, eventType, eventHandlerMethodsBuilder);
            }

            if (warningMessages.Count == 0) return true;

            warnings = new EventHandlerBuildWarnings(eventHandlerId, warningMessages);
            return false;
        }

        bool TryAddConventionHandlerMethods(
            IEnumerable<MethodInfo> methods,
            EventHandlerId eventHandlerId,
            IEventTypes eventTypes,
            EventHandlerMethodsBuilder eventHandlerMethodsBuilder,
            out EventHandlerBuildWarnings warnings)
        {
            warnings = default;
            var warningMessages = new List<string>();
            foreach (var method in methods.Where(_ => !IsDecoratedHandlerMethod(_) && _.Name == MethodName))
            {
                var shouldAddHandler = true;
                if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {_eventHandlerType} has no parameters. An event handler method should take in as paramters an event and an {typeof(EventContext).Name}");
                    shouldAddHandler = false;
                }

                if (eventParameterType == typeof(object))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {_eventHandlerType} cannot handle an untyped event when not decorated with [{typeof(HandlesAttribute).Name}]");
                    shouldAddHandler = false;
                }

                if (!eventTypes.HasFor(eventParameterType))
                {
                    warningMessages.Add($"Event handler method {method} on event handler {_eventHandlerType} handles event of type {eventParameterType}, but it is not associated to any event type");
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, eventHandlerId, out var parameterWarnings))
                {
                    warningMessages.AddRange(parameterWarnings.Warnings);
                    shouldAddHandler = false;
                }

                if (shouldAddHandler) AddTypedHandleSignatureToBuilder(eventParameterType, method, eventHandlerMethodsBuilder);
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
                warnings.Append($"Event handler method {method} on event handler {_eventHandlerType} needs to have two parameters where the second parameter is {typeof(EventContext)}");
            }

            if (!MethodHasNoExtraParameters(method))
            {
                warnings.Append($"Event handler method {method} on event handler {_eventHandlerType} needs to only have two parameters where the first is the event to handle and the second is {typeof(EventContext)}");
            }

            if (!MethodReturnsVoid(method) && !MethodReturnsTask(method))
            {
                warnings.Append($"Event handler method {method} on event handler {_eventHandlerType} needs to return either {typeof(void)} or {typeof(Task)}");
            }

            if (warnings.Count == 0) return true;
            buildWarnings = new EventHandlerBuildWarnings(eventHandlerId, warnings);
            return false;
        }

        void AddUntypedHandleSignatureToBuilder(MethodInfo method, EventType eventType, EventHandlerMethodsBuilder builder)
        {
            var eventHandlerSignatureType = method.ReturnType == typeof(Task) ? typeof(EventHandlerSignature) : typeof(VoidEventHandlerSignature);
            var eventHandlerSignature = Delegate.CreateDelegate(eventHandlerSignatureType, _eventHandlerInstance, method);
            var builderHandleMethod = typeof(EventHandlerMethodsBuilder)
                                        .GetMethod(nameof(EventHandlerMethodsBuilder.Handle), new[] { typeof(EventType), eventHandlerSignatureType });
            if (eventHandlerSignature is EventHandlerSignature signature) builder.Handle(eventType, signature);
            else if (eventHandlerSignature is VoidEventHandlerSignature voidSignature) builder.Handle(eventType, voidSignature);
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