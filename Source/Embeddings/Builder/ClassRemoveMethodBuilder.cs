
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
    /// Builder for building <see cref="ClassRemoveMethod{TEmbedding}"/>.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    public class ClassRemoveMethodBuilder<TEmbedding> : ClassMethodBuilder<TEmbedding>
        where TEmbedding : class, new()
    {
        const string RemoveMethodName = "Remove";
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassRemoveMethodBuilder{TEmbedding}"/> class.
        /// </summary>
        /// <param name="embeddingId">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="logger">The logger.</param>
        public ClassRemoveMethodBuilder(EmbeddingId embeddingId, IEventTypes eventTypes, ILogger logger)
            : base(embeddingId, eventTypes)
        {
            _logger = logger;
        }

        /// <summary>
        /// Try to build an <see cref="IRemoveMethod{TEmbedding}"/>.
        /// </summary>
        /// <param name="method">The out of the method.</param>
        /// <returns>A bool indicating whether the build succeeded.</returns>
        public bool TryBuild(out IRemoveMethod<TEmbedding> method)
        {
            var allMethods = EmbeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            if (!TryAddDecoratedRemoveMethod(allMethods, out method)
                && !TryAddConventionRemoveMethod(allMethods, out method))
            {
                _logger.LogWarning(
                    "No compare method defined for embedding {EmbeddingType} with id {EmbeddingId}. An embeddin needs to have one compare method, which is either named {CompareName} or attributed with [{RemoveAttribute}].",
                    EmbeddingType,
                    Embedding,
                    RemoveMethodName,
                    nameof(RemoveAttribute));
                return false;
            }

            return true;
        }

        bool TryAddDecoratedRemoveMethod(
            IEnumerable<MethodInfo> allMethods,
            out IRemoveMethod<TEmbedding> removeMethod)
        {
            removeMethod = default;
            if (allMethods.Count(IsDecoratedRemoveMethod) > 1)
            {
                _logger.LogWarning(
                    "More than one Remove method attributed on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one Remove method.",
                    EmbeddingType,
                    Embedding);
                return false;
            }

            var decoratedMethod = allMethods.SingleOrDefault(IsDecoratedRemoveMethod);
            if (decoratedMethod == default)
            {
                return false;
            }

            if (!RemoveMethodParametersAreOkay(decoratedMethod))
            {
                return false;
            }

            removeMethod = CreateRemoveMethod(decoratedMethod);
            return false;
        }

        bool RemoveMethodParametersAreOkay(MethodInfo method)
        {
            var okay = true;
            if (!SecondMethodParameterIsEmbeddingContext(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Remove method {Method} on embedding {EmbeddingType} needs to have two parameters, where the second parameters is {EmbeddingContext}",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            if (!RemoveMethodHasNoExtraParameters(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Remove method {Method} on embedding {EmbeddingType} needs to have two parameters, where the first one is the received state and the second is {EmbeddingContext}",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            if (!MethodReturnsEnumerableObject(method) && !MethodReturnsObject(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Remove method {Method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>, which represent events.",
                    method,
                    EmbeddingType);
            }

            return okay;
        }

        bool TryAddConventionRemoveMethod(
            IEnumerable<MethodInfo> allMethods,
            out IRemoveMethod<TEmbedding> removeMethod)
        {
            removeMethod = default;

            if (allMethods.Count(_ => IsDecoratedRemoveMethod(_) || _.Name == RemoveMethodName) > 1)
            {
                _logger.LogWarning(
                    "More than one Remove method on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one Remove method called {RemoveName} or attributed with [{RemoveAttribute}}.",
                    EmbeddingType,
                    Embedding,
                    RemoveMethodName,
                    nameof(RemoveAttribute));
                return false;
            }

            var decoratedMethod = allMethods
                .SingleOrDefault(_ => !IsDecoratedRemoveMethod(_) && _.Name == RemoveMethodName);
            if (decoratedMethod == default)
            {
                return false;
            }

            if (!RemoveMethodParametersAreOkay(decoratedMethod))
            {
                return false;
            }

            removeMethod = CreateRemoveMethod(decoratedMethod);
            return true;
        }

        IRemoveMethod<TEmbedding> CreateRemoveMethod(MethodInfo method)
        {
            var compareSignatureType = GetRemoveSignatureType(method);
            var compareSignature = method.CreateDelegate(compareSignatureType.MakeGenericType(EmbeddingType), null);
            return Activator.CreateInstance(
                typeof(ClassRemoveMethod<>).MakeGenericType(EmbeddingType),
                compareSignature) as IRemoveMethod<TEmbedding>;
        }

        Type GetRemoveSignatureType(MethodInfo method)
        {
            if (MethodReturnsEnumerableObject(method)) return typeof(RemoveMethodEnumerableReturnSignature<>);
            if (MethodReturnsObject(method)) return typeof(RemoveMethodSignature<>);
            throw new InvalidRemoveMethodReturnType(method.ReturnType);
        }

        bool IsDecoratedRemoveMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(RemoveAttribute), true).FirstOrDefault() != default;

        bool RemoveMethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;
    }
}
