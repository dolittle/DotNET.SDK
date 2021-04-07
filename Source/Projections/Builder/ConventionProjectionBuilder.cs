// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Projections.Internal;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Methods for building <see cref="IProjection{TReadModel}"/> instances by convention from an instantiated projection class.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the read model.</typeparam>
    public class ConventionProjectionBuilder<TProjection> : ICanBuildAndRegisterAProjection
        where TProjection : class, new()
    {
        const string MethodName = "On";
        readonly Type _projectionType = typeof(TProjection);

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            var logger = loggerFactory.CreateLogger(GetType());
            logger.LogDebug("Building projection from type {ProjectionType}", _projectionType);

            if (!HasParameterlessConstructor())
            {
                logger.LogWarning("The projection class {ProjectionType} has no deafult/parameterless constructor", _projectionType);
                return;
            }

            if (HasMoreThanOneConstructor())
            {
                logger.LogWarning("The projection class {ProjectionType} has more than one constructor. It must only have one, parameterless, constructor", _projectionType);
                return;
            }

            if (!TryGetProjectionInformation(out var projectionId, out var scopeId))
            {
                logger.LogWarning("The projection class {ProjectionType} needs to be decorated with an [{ProjectionAttribute}]", _projectionType, typeof(ProjectionAttribute).Name);
                return;
            }

            logger.LogTrace(
                "Building projection {Projection} in scope {ScopeId} from type {ProjectionType}",
                projectionId,
                scopeId,
                _projectionType);

            var eventTypesToMethods = new Dictionary<EventType, IProjectionMethod<TProjection>>();

            if (!TryBuildOnMethods(projectionId, eventTypes, eventTypesToMethods, logger))
            {
                return;
            }

            var projection = new Projection<TProjection>(projectionId, scopeId, eventTypesToMethods);
            var projectionProcessor = new ProjectionsProcessor<TProjection>(
                projection,
                processingConverter,
                loggerFactory.CreateLogger<ProjectionsProcessor<TProjection>>());

            eventProcessors.Register(
                projectionProcessor,
                new ProjectionsProtocol(),
                cancellation);
        }

        bool HasMoreThanOneConstructor()
            => _projectionType.GetConstructors().Length > 1;

        bool HasParameterlessConstructor()
            => _projectionType.GetConstructors().Any(t => t.GetParameters().Length == 0);

        bool TryBuildOnMethods(
            ProjectionId projectionId,
            IEventTypes eventTypes,
            IDictionary<EventType, IProjectionMethod<TProjection>> eventTypesToMethods,
            ILogger logger)
        {
            var allMethods = _projectionType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            var hasWrongMethods = false;
            if (!TryAddDecoratedOnMethods(allMethods, projectionId, eventTypesToMethods, logger))
            {
                hasWrongMethods = true;
            }

            if (!TryAddConventionOnMethods(allMethods, projectionId, eventTypes, eventTypesToMethods, logger))
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
                    "There are no projection methods to register in projection {ProjectionType}. A projection method either needs to be decorated with [{OnAttribute}] or have the name {MethodName}",
                    _projectionType,
                    typeof(OnAttribute).Name,
                    MethodName);
                return false;
            }

            return true;
        }

        bool TryAddDecoratedOnMethods(
            IEnumerable<MethodInfo> methods,
            ProjectionId projectionId,
            IDictionary<EventType, IProjectionMethod<TProjection>> eventTypesToMethods,
            ILogger logger)
        {
            var allMethodsAdded = true;
            foreach (var method in methods.Where(IsDecoratedOnMethod))
            {
                var shouldAddHandler = true;
                var eventType = (method.GetCustomAttributes(typeof(OnAttribute), true)[0] as OnAttribute)?.EventType;

                if (!TryGetKeySelector(method, logger, out var keySelector))
                {
                    shouldAddHandler = false;
                }

                if (!TryGetEventParameterType(method, out var eventParameterType))
                {
                    logger.LogWarning(
                        "Projection method {Method} on projection {Projection} has no parameters, but is decorated with [{OnAttribute}]. A projection method should take in as parameters an event and a {ProjectionContext}",
                        method,
                        _projectionType,
                        typeof(OnAttribute).Name,
                        typeof(ProjectionContext).Name);
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, logger)) shouldAddHandler = false;

                if (eventParameterType != typeof(object))
                {
                    logger.LogWarning(
                        "Projection method {Method} on projection {ProjectionType} should only handle an event of type object",
                        method,
                        _projectionType);
                    shouldAddHandler = false;
                }

                if (!method.IsPublic)
                {
                    logger.LogWarning(
                        "Method {Method} on projection {ProjectionType} has the signature of an projection method, but is not public. Projection methods needs to be public",
                        method,
                        _projectionType);
                    shouldAddHandler = false;
                }

                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, CreateUntypedOnMethod(method, eventType, keySelector)))
                {
                    allMethodsAdded = false;
                    logger.LogWarning(
                        "Event type {EventType} is already handled in projection {Projection}",
                        eventType,
                        projectionId);
                }

                if (!shouldAddHandler) allMethodsAdded = false;
            }

            return allMethodsAdded;
        }

        bool TryAddConventionOnMethods(
            IEnumerable<MethodInfo> methods,
            ProjectionId projectionId,
            IEventTypes eventTypes,
            IDictionary<EventType, IProjectionMethod<TProjection>> eventTypesToMethods,
            ILogger logger)
        {
            var allMethodsAdded = true;
            foreach (var method in methods.Where(_ => !IsDecoratedOnMethod(_) && _.Name == MethodName))
            {
                var shouldAddHandler = true;

                if (!TryGetKeySelector(method, logger, out var keySelector))
                {
                    shouldAddHandler = false;
                }

                if (!TryGetEventParameterType(method, out var eventParameterType))
                {
                    logger.LogWarning(
                        "Projection method {Method} on projection {ProjectionType} has no parameters. An projection method should take in as paramters an event and an {ProjectionContext}",
                        method,
                        _projectionType,
                        typeof(ProjectionContext).Name);
                    shouldAddHandler = false;
                }

                if (eventParameterType == typeof(object))
                {
                    logger.LogWarning(
                        "Projection method {Method} on projection {ProjectionType} cannot handle an untyped event when not decorated with [{HandlesAttribute}]",
                        method,
                        _projectionType,
                        typeof(OnAttribute).Name);
                    shouldAddHandler = false;
                }

                if (!eventTypes.HasFor(eventParameterType))
                {
                    logger.LogWarning(
                        "Projection method {Method} on projection {ProjectionType} handles event of type {EventParameterType}, but it is not associated to any event type",
                        method,
                        _projectionType,
                        eventParameterType);
                    shouldAddHandler = false;
                }

                if (!ParametersAreOkay(method, logger)) shouldAddHandler = false;

                if (!method.IsPublic)
                {
                    logger.LogWarning(
                        "Method {Method} on projection {ProjectionType} has the signature of an projection method, but is not public. Projection methods needs to be public",
                        method,
                        _projectionType);
                    shouldAddHandler = false;
                }

                if (!shouldAddHandler) continue;

                var eventType = eventTypes.GetFor(eventParameterType);
                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, CreateTypedOnMethod(eventParameterType, method, keySelector)))
                {
                    allMethodsAdded = false;
                    logger.LogWarning(
                        "Event type {EventParameterType} is already handled in projection {ProjectionId}",
                        eventParameterType,
                        projectionId);
                }

                if (!shouldAddHandler) allMethodsAdded = false;
            }

            return allMethodsAdded;
        }

        IProjectionMethod<TProjection> CreateUntypedOnMethod(MethodInfo method, EventType eventType, KeySelector keySelector)
        {
            var projectionSignatureType = GetSignature(method);
            var projectionSignature = method.CreateDelegate(projectionSignatureType.MakeGenericType(_projectionType), null);
            return Activator.CreateInstance(
                typeof(ClassProjectionMethod<>).MakeGenericType(_projectionType),
                projectionSignature,
                eventType,
                keySelector) as IProjectionMethod<TProjection>;
        }

        IProjectionMethod<TProjection> CreateTypedOnMethod(Type eventParameterType, MethodInfo method, KeySelector keySelector)
        {
            var projectionSignatureGenericTypeDefinition = GetTypedSignature(method);
            var projectionSignatureType = projectionSignatureGenericTypeDefinition.MakeGenericType(_projectionType, eventParameterType);
            var projectionSignature = method.CreateDelegate(projectionSignatureType, null);

            return Activator.CreateInstance(
                typeof(TypedClassProjectionMethod<,>).MakeGenericType(_projectionType, eventParameterType),
                projectionSignature,
                keySelector) as IProjectionMethod<TProjection>;
        }

        Type GetSignature(MethodInfo method)
        {
            if (MethodReturnsTask(method)) return typeof(TaskProjectionMethodSignature<>);
            if (MethodReturnsTaskResultType(method)) return typeof(TaskResultProjectionMethodSignature<>);
            if (MethodReturnsVoid(method)) return typeof(SyncProjectionMethodSignature<>);
            if (MethodReturnsResultType(method)) return typeof(SyncResultProjectionMethodSignature<>);
            throw new InvalidProjectionMethodReturnType(method.ReturnType);
        }

        Type GetTypedSignature(MethodInfo method)
        {
            if (MethodReturnsTask(method)) return typeof(TaskProjectionMethodSignature<,>);
            if (MethodReturnsTaskResultType(method)) return typeof(TaskResultProjectionMethodSignature<,>);
            if (MethodReturnsVoid(method)) return typeof(SyncProjectionMethodSignature<,>);
            if (MethodReturnsResultType(method)) return typeof(SyncResultProjectionMethodSignature<,>);
            throw new InvalidProjectionMethodReturnType(method.ReturnType);
        }

        bool ParametersAreOkay(MethodInfo method, ILogger logger)
        {
            var okay = true;
            if (!SecondMethodParameterIsProjectionContext(method))
            {
                okay = false;
                logger.LogWarning(
                    "Projection method {Method} on projection {ProjectionType} needs to have two parameters where the second parameter is {ProjectionContext}",
                    method,
                    _projectionType,
                    typeof(ProjectionContext));
            }

            if (!MethodHasNoExtraParameters(method))
            {
                okay = false;
                logger.LogWarning(
                    "Projection method {Method} on projection {ProjectionType} needs to only have two parameters where the first is the event to handle and the second is {ProjectionContext}",
                    method,
                    _projectionType,
                    typeof(ProjectionContext));
            }

            if (MethodReturnsAsyncVoid(method)
                || (!MethodReturnsVoid(method) && !MethodReturnsResultType(method) && !MethodReturnsTask(method) && !MethodReturnsTaskResultType(method)))
            {
                okay = false;
                logger.LogWarning(
                    "Projection method {Method} on projection {ProjectionType} needs to return either {Void}, {ResultType}, {Task}, {TaskResultType}",
                    method,
                    _projectionType,
                    typeof(void),
                    typeof(ProjectionResultType),
                    typeof(Task),
                    typeof(Task<ProjectionResultType>));
            }

            return okay;
        }

        bool TryGetKeySelector(MethodInfo method, ILogger logger, out KeySelector keySelector)
        {
            keySelector = null;
            var attributes = method
                                .GetCustomAttributes()
                                .Where(_ => _ is IKeySelectorAttribute)
                                .Select(_ => _ as IKeySelectorAttribute);
            if (attributes.Count() > 1)
            {
                logger.LogWarning(
                    "Method {Method} on projection {ProjectionType} has more than one key selector attributes. Use only one",
                    method,
                    _projectionType);
                return false;
            }

            if (!attributes.Any())
            {
                logger.LogWarning(
                    "Method {Method} on projection {ProjectionType} has no key selector attribute",
                    method,
                    _projectionType);
                return false;
            }

            keySelector = attributes.Single().KeySelector;

            return true;
        }

        bool TryGetProjectionInformation(out ProjectionId projectionId, out ScopeId scopeId)
        {
            projectionId = default;
            scopeId = default;
            var projection = _projectionType.GetCustomAttributes(typeof(ProjectionAttribute), true).FirstOrDefault() as ProjectionAttribute;
            if (projection == default) return false;

            projectionId = projection.Identifier;
            scopeId = projection.Scope;
            return true;
        }

        bool TryGetEventParameterType(MethodInfo method, out Type type)
        {
            type = default;
            if (method.GetParameters().Length == 0) return false;

            type = method.GetParameters()[0].ParameterType;
            return true;
        }

        bool IsDecoratedOnMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(OnAttribute), true).FirstOrDefault() != default;

        bool SecondMethodParameterIsProjectionContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(ProjectionContext);

        bool MethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;

        bool MethodReturnsTask(MethodInfo method)
            => method.ReturnType == typeof(Task);

        bool MethodReturnsTaskResultType(MethodInfo method)
            => method.ReturnType == typeof(Task<ProjectionResultType>);

        bool MethodReturnsVoid(MethodInfo method)
            => method.ReturnType == typeof(void);

        bool MethodReturnsResultType(MethodInfo method)
            => method.ReturnType == typeof(ProjectionResultType);

        bool MethodReturnsAsyncVoid(MethodInfo method)
        {
            var asyncAttribute = typeof(AsyncStateMachineAttribute);
            var isAsyncMethod = (AsyncStateMachineAttribute)method.GetCustomAttribute(asyncAttribute) != null;
            return isAsyncMethod && MethodReturnsVoid(method);
        }
    }
}
