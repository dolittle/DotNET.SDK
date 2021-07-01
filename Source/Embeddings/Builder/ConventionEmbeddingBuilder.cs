// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Embeddings.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Methods for building <see cref="IProjection{TReadModel}"/> instances by convention from an instantiated projection class.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the read model.</typeparam>
    public class ConventionEmbeddingBuilder<TEmbedding> : ICanBuildAndRegisterAnEmbedding
        where TEmbedding : class, new()
    {
        const string ProjectionMethodName = "On";
        const string CompareMethodName = "Compare";
        const string RemoveMethodName = "Remove";
        readonly Type _embeddingType = typeof(TEmbedding);

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IConvertEventsToProtobuf processingConverter,
            IConvertProjectionsToSDK projectionsConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            var logger = loggerFactory.CreateLogger(GetType());
            logger.LogDebug("Building embedding from type {EmbeddingType}", _embeddingType);

            if (!HasParameterlessConstructor())
            {
                logger.LogWarning("The embedding class {EmbeddingType} has no default/parameterless constructor", _embeddingType);
                return;
            }

            if (HasMoreThanOneConstructor())
            {
                logger.LogWarning("The embedding class {EmbeddingType} has more than one constructor. It must only have one, parameterless, constructor", _embeddingType);
                return;
            }

            if (!TryGetEmbeddingId(out var embeddingId))
            {
                logger.LogWarning("The embedding class {EmbeddingType} needs to be decorated with an [{EmbeddingAttribute}]", _embeddingType, typeof(EmbeddingAttribute).Name);
                return;
            }

            logger.LogTrace(
                "Building embedding {Embedding} from type {EmbeddingType}",
                embeddingId,
                _embeddingType);

            var eventTypesToMethods = new Dictionary<EventType, IOnMethod<TEmbedding>>();

            if (!TryBuildProjectionMethods(embeddingId, eventTypes, eventTypesToMethods, logger))
            {
                return;
            }

            TryBuildCompareMethod(embeddingId, logger, out var compareMethod);
            TryBuildRemoveMethod(embeddingId, logger, out var removeMethod);

            var embedding = new Embedding<TEmbedding>(
                embeddingId,
                eventTypes,
                eventTypesToMethods,
                compareMethod,
                removeMethod);
            var embeddingsProcessor = new EmbeddingsProcessor<TEmbedding>(
                embedding,
                processingConverter,
                projectionsConverter,
                eventTypes,
                loggerFactory.CreateLogger<EmbeddingsProcessor<TEmbedding>>());

            eventProcessors.Register(
                embeddingsProcessor,
                new EmbeddingsProtocol(),
                cancellation);
        }

        bool TryBuildCompareMethod(EmbeddingId embeddingId, ILogger logger, out ICompareMethod<TEmbedding> compareMethod)
        {
            var allMethods = _embeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            if (!TryAddDecoratedCompareMethod(allMethods, embeddingId, logger, out compareMethod)
                && !TryAddConventionCompareMethod(allMethods, embeddingId, logger, out compareMethod))
            {
                logger.LogWarning(
                    "No compare method defined for embedding {EmbeddingType} with id {EmbeddingId}. An embeddin needs to have one compare method, which is either named {CompareName} or attributed with [{CompareAttribute}].",
                    _embeddingType,
                    embeddingId,
                    CompareMethodName,
                    nameof(CompareAttribute));
                return false;
            }

            return true;
        }

        bool TryAddDecoratedCompareMethod(
            IEnumerable<MethodInfo> allMethods,
            EmbeddingId embeddingId,
            ILogger logger,
            out ICompareMethod<TEmbedding> compareMethod)
        {
            compareMethod = default;
            if (allMethods.Count(IsDecoratedCompareMethod) > 1)
            {
                logger.LogWarning(
                    "More than one Compare method attributed on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one Compare method.",
                    _embeddingType,
                    embeddingId);
                return false;
            }

            var decoratedMethod = allMethods.SingleOrDefault(IsDecoratedCompareMethod);
            if (decoratedMethod == default)
            {
                return false;
            }

            if (!CompareMethodParametersAreOkay(decoratedMethod, logger))
            {
                return false;
            }

            compareMethod = CreateCompareMethod(decoratedMethod);
            return false;
        }

        bool CompareMethodParametersAreOkay(MethodInfo method, ILogger logger)
        {
            var okay = true;
            if (!SecondMethodParameterIsEmbeddingContext(method))
            {
                okay = false;
                logger.LogWarning(
                    "Compare method {Method} on embedding {EmbeddingType} needs to have two parameters, where the second parameters is {EmbeddingContext}",
                    method,
                    _embeddingType,
                    typeof(EmbeddingContext));
            }

            if (!CompareMethodHasNoExtraParameters(method))
            {
                okay = false;
                logger.LogWarning(
                    "Compare method {Method} on embedding {EmbeddingType} needs to have two parameters, where the first one is the received state and the second is {EmbeddingContext}",
                    method,
                    _embeddingType,
                    typeof(EmbeddingContext));
            }

            if (!MethodReturnsEnumerableObject(method) && !MethodReturnsObject(method))
            {
                okay = false;
                logger.LogWarning(
                    "Compare method {Method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.",
                    method,
                    _embeddingType);
            }

            return okay;
        }

        bool TryAddConventionCompareMethod(
            IEnumerable<MethodInfo> allMethods,
            EmbeddingId embeddingId,
            ILogger logger,
            out ICompareMethod<TEmbedding> compareMethod)
        {
            compareMethod = default;

            if (allMethods.Count(_ => IsDecoratedCompareMethod(_) || _.Name == CompareMethodName) > 1)
            {
                logger.LogWarning(
                    "More than one Compare method on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one Compare method called {CompareName} or attributed with [{CompareAttribute}}.",
                    _embeddingType,
                    embeddingId,
                    CompareMethodName,
                    nameof(CompareAttribute));
                return false;
            }

            var decoratedMethod = allMethods
                .SingleOrDefault(_ => !IsDecoratedCompareMethod(_) && _.Name == CompareMethodName);
            if (decoratedMethod == default)
            {
                return false;
            }

            if (!CompareMethodParametersAreOkay(decoratedMethod, logger))
            {
                return false;
            }

            compareMethod = CreateCompareMethod(decoratedMethod);
            return false;
        }

        bool TryBuildRemoveMethod(EmbeddingId embeddingId, ILogger logger, out IRemoveMethod<TEmbedding> removeMethod)
        {
            removeMethod = default;

            return true;
        }

        bool HasMoreThanOneConstructor()
            => _embeddingType.GetConstructors().Length > 1;

        bool HasParameterlessConstructor()
            => _embeddingType.GetConstructors().Any(t => t.GetParameters().Length == 0);

        bool TryBuildProjectionMethods(
            EmbeddingId embeddingId,
            IEventTypes eventTypes,
            IDictionary<EventType, IOnMethod<TEmbedding>> eventTypesToMethods,
            ILogger logger)
        {
            var allMethods = _embeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            var hasWrongMethods = false;
            if (!TryAddDecoratedProjectionMethods(allMethods, embeddingId, eventTypesToMethods, logger))
            {
                hasWrongMethods = true;
            }

            if (!TryAddConventionProjectionMethods(allMethods, embeddingId, eventTypes, eventTypesToMethods, logger))
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
                    "There are no projection methods to register in embedding {EmbeddingType}. A projection method either needs to be decorated with [{OnAttribute}] or have the name {MethodName}",
                    _embeddingType,
                    nameof(OnAttribute),
                    ProjectionMethodName);
                return false;
            }

            return true;
        }

        bool TryAddDecoratedProjectionMethods(
            IEnumerable<MethodInfo> methods,
            EmbeddingId embeddingId,
            IDictionary<EventType, IOnMethod<TEmbedding>> eventTypesToMethods,
            ILogger logger)
        {
            var allMethodsAdded = true;
            foreach (var method in methods.Where(IsDecoratedOnMethod))
            {
                var shouldAddHandler = true;
                var eventType = (method.GetCustomAttributes(typeof(OnAttribute), true)[0] as OnAttribute)?.EventType;

                if (!TryGetEventParameterType(method, out var eventParameterType))
                {
                    logger.LogWarning(
                        "Projection method {Method} on embedding {Embedding} has no parameters, but is decorated with [{OnAttribute}]. An on method should take in as parameters an event and a {EmbeddingProjectContext}",
                        method,
                        _embeddingType,
                        nameof(OnAttribute),
                        nameof(EmbeddingProjectContext));
                    shouldAddHandler = false;
                }

                if (!OnMethodParametersAreOkay(method, logger)) shouldAddHandler = false;

                if (eventParameterType != typeof(object))
                {
                    logger.LogWarning(
                        "Projection method {Method} on embedding {EmbeddingType} should only handle an event of type object",
                        method,
                        _embeddingType);
                    shouldAddHandler = false;
                }

                if (!method.IsPublic)
                {
                    logger.LogWarning(
                        "Method {Method} on embedding {EmbeddingType} has the signature of an projection method, but is not public. Projection methods needs to be public",
                        method,
                        _embeddingType);
                    shouldAddHandler = false;
                }

                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, CreateUntypedOnMethod(method, eventType)))
                {
                    allMethodsAdded = false;
                    logger.LogWarning(
                        "Event type {EventType} is already handled in projection {Projection}",
                        eventType,
                        embeddingId);
                }

                if (!shouldAddHandler) allMethodsAdded = false;
            }

            return allMethodsAdded;
        }

        bool TryAddConventionProjectionMethods(
            IEnumerable<MethodInfo> methods,
            EmbeddingId embeddingId,
            IEventTypes eventTypes,
            IDictionary<EventType, IOnMethod<TEmbedding>> eventTypesToMethods,
            ILogger logger)
        {
            var allMethodsAdded = true;
            foreach (var method in methods.Where(_ => !IsDecoratedOnMethod(_) && _.Name == ProjectionMethodName))
            {
                var shouldAddHandler = true;

                if (!TryGetEventParameterType(method, out var eventParameterType))
                {
                    logger.LogWarning(
                        "Projection method {Method} on embedding {EmbeddingType} has no parameters. An projection method should take in as paramters an event and an {ProjectionContext}",
                        method,
                        _embeddingType,
                        nameof(ProjectionContext));
                    shouldAddHandler = false;
                }

                if (eventParameterType == typeof(object))
                {
                    logger.LogWarning(
                        "Projection method {Method} on embedding {EmbeddingType} cannot handle an untyped event when not decorated with [{HandlesAttribute}]",
                        method,
                        _embeddingType,
                        nameof(OnAttribute));
                    shouldAddHandler = false;
                }

                if (!eventTypes.HasFor(eventParameterType))
                {
                    logger.LogWarning(
                        "Projection method {Method} on embedding {EmbeddingType} handles event of type {EventParameterType}, but it is not associated to any event type",
                        method,
                        _embeddingType,
                        eventParameterType);
                    shouldAddHandler = false;
                }

                if (!OnMethodParametersAreOkay(method, logger)) shouldAddHandler = false;

                if (!method.IsPublic)
                {
                    logger.LogWarning(
                        "Method {Method} on embedding {EmbeddingType} has the signature of an projection method, but is not public. Projection methods needs to be public",
                        method,
                        _embeddingType);
                    shouldAddHandler = false;
                }

                if (!shouldAddHandler) continue;

                var eventType = eventTypes.GetFor(eventParameterType);
                if (shouldAddHandler && !eventTypesToMethods.TryAdd(eventType, CreateTypedOnMethod(eventParameterType, method)))
                {
                    allMethodsAdded = false;
                    logger.LogWarning(
                        "Event type {EventParameterType} is already handled in embedding {EmbeddingId}",
                        eventParameterType,
                        embeddingId);
                }

                if (!shouldAddHandler) allMethodsAdded = false;
            }

            return allMethodsAdded;
        }

        ICompareMethod<TEmbedding> CreateCompareMethod(MethodInfo method)
        {
            var compareSignatureType = GetCompareSignatureType(method);
            var compareSignature = method.CreateDelegate(compareSignatureType.MakeGenericType(_embeddingType), null);
            return Activator.CreateInstance(
                typeof(ClassCompareMethod<>).MakeGenericType(_embeddingType),
                compareSignature) as ICompareMethod<TEmbedding>;
        }

        IOnMethod<TEmbedding> CreateUntypedOnMethod(MethodInfo method, EventType eventType)
        {
            var onSignatureType = GetOnMethodSignature(method);
            var onSignature = method.CreateDelegate(onSignatureType.MakeGenericType(_embeddingType), null);
            return Activator.CreateInstance(
                typeof(ClassOnMethod<>).MakeGenericType(_embeddingType),
                onSignature,
                eventType) as IOnMethod<TEmbedding>;
        }

        IOnMethod<TEmbedding> CreateTypedOnMethod(Type eventParameterType, MethodInfo method)
        {
            var onSignatureGenericTypeDefinition = GetTypedOnMethodSignature(method);
            var onSignatureType = onSignatureGenericTypeDefinition.MakeGenericType(_embeddingType, eventParameterType);
            var onSignature = method.CreateDelegate(onSignatureType, null);

            return Activator.CreateInstance(
                typeof(TypedClassOnMethod<,>).MakeGenericType(_embeddingType, eventParameterType),
                onSignature) as IOnMethod<TEmbedding>;
        }

        Type GetCompareSignatureType(MethodInfo method)
        {
            if (MethodReturnsEnumerableObject(method)) return typeof(CompareMethodEnumerableReturnSignature<>);
            if (MethodReturnsObject(method)) return typeof(CompareMethodSignature<>);
            throw new InvalidCompareMethodReturnType(method.ReturnType);
        }

        Type GetRemoveSignatureType(MethodInfo method)
        {
            if (MethodReturnsEnumerableObject(method)) return typeof(RemoveMethodEnumerableReturnSignature<>);
            return typeof(RemoveMethodSignature<>);
        }

        Type GetOnMethodSignature(MethodInfo method)
        {
            if (MethodReturnsTask(method)) return typeof(TaskOnMethodSignature<>);
            if (MethodReturnsTaskResultType(method)) return typeof(TaskResultOnMethodSignature<>);
            if (MethodReturnsVoid(method)) return typeof(SyncOnMethodSignature<>);
            if (MethodReturnsResultType(method)) return typeof(SyncResultOnMethodSignature<>);
            throw new InvalidProjectionMethodReturnType(method.ReturnType);
        }

        Type GetTypedOnMethodSignature(MethodInfo method)
        {
            if (MethodReturnsTask(method)) return typeof(TaskOnMethodSignature<,>);
            if (MethodReturnsTaskResultType(method)) return typeof(TaskResultOnMethodSignature<,>);
            if (MethodReturnsVoid(method)) return typeof(SyncOnMethodSignature<,>);
            if (MethodReturnsResultType(method)) return typeof(SyncResultOnMethodSignature<,>);
            throw new InvalidProjectionMethodReturnType(method.ReturnType);
        }

        bool OnMethodParametersAreOkay(MethodInfo method, ILogger logger)
        {
            var okay = true;
            if (!SecondMethodParameterIsEmbeddingProjectionContext(method))
            {
                okay = false;
                logger.LogWarning(
                    "Projection method {Method} on embedding {EmbeddingType} needs to have two parameters where the second parameter is {EmbeddingProjectContext}",
                    method,
                    _embeddingType,
                    typeof(EmbeddingProjectContext));
            }

            if (!ProjectionMethodHasNoExtraParameters(method))
            {
                okay = false;
                logger.LogWarning(
                    "Projection method {Method} on embeddingg {EmbeddingType} needs to only have two parameters where the first is the event to handle and the second is {EmbeddingProjectContext}",
                    method,
                    _embeddingType,
                    typeof(EmbeddingProjectContext));
            }

            if (MethodReturnsAsyncVoid(method)
                || (!MethodReturnsVoid(method) && !MethodReturnsResultType(method) && !MethodReturnsTask(method) && !MethodReturnsTaskResultType(method)))
            {
                okay = false;
                logger.LogWarning(
                    "Projection method {Method} on embedding {EmbeddingType} needs to return either {Void}, {ResultType}, {Task}, {TaskResultType}",
                    method,
                    _embeddingType,
                    typeof(void),
                    typeof(ProjectionResultType),
                    typeof(Task),
                    typeof(Task<ProjectionResultType>));
            }

            return okay;
        }

        bool TryGetEmbeddingId(out EmbeddingId embeddingId)
        {
            embeddingId = default;
            var embedding = _embeddingType.GetCustomAttributes(typeof(EmbeddingAttribute), true).FirstOrDefault() as EmbeddingAttribute;
            if (embedding == default) return false;

            embeddingId = embedding.Identifier;
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

        bool SecondMethodParameterIsEmbeddingProjectionContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EmbeddingProjectContext);

        bool ProjectionMethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;

        bool MethodReturnsEnumerableObject(MethodInfo method)
            => typeof(System.Collections.IEnumerable).IsAssignableFrom(method.ReturnType);

        bool MethodReturnsObject(MethodInfo method)
            => typeof(object).IsAssignableFrom(method.ReturnType)
                && !MethodReturnsTask(method)
                && !method.ReturnType.IsGenericType;

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

        bool IsDecoratedCompareMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(CompareAttribute), true).FirstOrDefault() != default;
        bool SecondMethodParameterIsEmbeddingContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EmbeddingContext);
        bool CompareMethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;
    }
}
