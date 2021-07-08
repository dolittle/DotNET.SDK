// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Builder for building <see cref="ClassUpdateMethod{TEmbedding}"/>.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    public class ClassUpdateMethodBuilder<TEmbedding> : ClassMethodBuilder<TEmbedding>
        where TEmbedding : class, new()
    {
        const string UpdateMethodName = "ResolveUpdateToEvents";
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassUpdateMethodBuilder{TEmbedding}"/> class.
        /// </summary>
        /// <param name="embeddingId">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="logger">The logger.</param>
        public ClassUpdateMethodBuilder(EmbeddingId embeddingId, IEventTypes eventTypes, ILogger logger)
            : base(embeddingId, eventTypes)
        {
            _logger = logger;
        }

        /// <summary>
        /// Try to build an <see cref="IUpdateMethod{TEmbedding}"/>.
        /// </summary>
        /// <param name="method">The out of the method.</param>
        /// <returns>A bool indicating whether the build succeeded.</returns>
        public bool TryBuild(out IUpdateMethod<TEmbedding> method)
        {
            var allMethods = EmbeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            if (!TryAddDecoratedUpdateMethod(allMethods, out method)
                && !TryAddConventionUpdateMethod(allMethods, out method))
            {
                _logger.LogWarning(
                    "No update method defined for embedding {EmbeddingType} with id {EmbeddingId}. An embedding needs to have one update method, which is either named {UpdateName} or attributed with [{UpdateAttribute}].",
                    EmbeddingType,
                    Embedding,
                    UpdateMethodName,
                    nameof(ResolveUpdateToEventsAttribute));
                return false;
            }

            return true;
        }

        bool TryAddDecoratedUpdateMethod(
            IEnumerable<MethodInfo> allMethods,
            out IUpdateMethod<TEmbedding> updateMethod)
        {
            updateMethod = default;
            if (allMethods.Count(IsDecoratedUpdateMethod) > 1)
            {
                _logger.LogWarning(
                    "More than one update method attributed on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one update method.",
                    EmbeddingType,
                    Embedding);
                return false;
            }

            var decoratedMethod = allMethods.SingleOrDefault(IsDecoratedUpdateMethod);
            if (!UpdateMethodIsOkay(decoratedMethod))
            {
                return false;
            }

            updateMethod = CreateUpdateMethod(decoratedMethod);
            return true;
        }

        bool TryAddConventionUpdateMethod(
           IEnumerable<MethodInfo> allMethods,
           out IUpdateMethod<TEmbedding> updateMethod)
        {
            updateMethod = default;

            if (allMethods.Count(_ => IsDecoratedUpdateMethod(_) || _.Name == UpdateMethodName) > 1)
            {
                _logger.LogWarning(
                    "More than one update method on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one update method called {UpdateName} or attributed with [{UpdateAttribute}}.",
                    EmbeddingType,
                    Embedding,
                    UpdateMethodName,
                    nameof(ResolveUpdateToEventsAttribute));
                return false;
            }

            var conventionMethod = allMethods
                .SingleOrDefault(_ => !IsDecoratedUpdateMethod(_) && _.Name == UpdateMethodName);
            if (!UpdateMethodIsOkay(conventionMethod))
            {
                return false;
            }

            updateMethod = CreateUpdateMethod(conventionMethod);
            return true;
        }

        bool UpdateMethodIsOkay(MethodInfo method)
        {
            if (method == default)
            {
                return false;
            }

            if (!UpdateMethodParametersAreOkay(method))
            {
                return false;
            }

            if (MethodReturnsTaskOrVoid(method))
            {
                _logger.LogWarning(
                    "Update method {Method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.",
                    method,
                    EmbeddingType);
                return false;
            }

            return true;
        }

        bool UpdateMethodParametersAreOkay(MethodInfo method)
        {
            var okay = true;
            if (!SecondMethodParameterIsEmbeddingContext(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Update method {Method} on embedding {EmbeddingType} needs to have two parameters, where the second parameters is {EmbeddingContext}",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            if (!UpdateMethodHasNoExtraParameters(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Update method {Method} on embedding {EmbeddingType} needs to have two parameters, where the first one is the received state and the second is {EmbeddingContext}",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            return okay;
        }

        IUpdateMethod<TEmbedding> CreateUpdateMethod(MethodInfo method)
        {
            var updateSignatureType = GetUpdateSignatureType(method);
            var updateSignature = method.CreateDelegate(updateSignatureType.MakeGenericType(EmbeddingType), null);
            return Activator.CreateInstance(
                typeof(ClassUpdateMethod<>).MakeGenericType(EmbeddingType),
                updateSignature) as IUpdateMethod<TEmbedding>;
        }

        Type GetUpdateSignatureType(MethodInfo method)
        {
            if (MethodReturnsTaskOrVoid(method))
            {
                throw new InvalidUpdateMethodReturnType(method.ReturnType);
            }

            if (MethodReturnsEnumerableObject(method))
            {
                return typeof(UpdateMethodEnumerableReturnSignature<>);
            }

            return typeof(UpdateMethodSignature<>);
        }

        bool SecondMethodParameterIsEmbeddingContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EmbeddingContext);

        bool IsDecoratedUpdateMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(ResolveUpdateToEventsAttribute), true).FirstOrDefault() != default;

        bool UpdateMethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;
    }
}
