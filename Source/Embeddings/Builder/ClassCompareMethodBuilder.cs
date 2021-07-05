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
    /// Builder for building <see cref="ClassCompareMethod{TEmbedding}"/>.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    public class ClassCompareMethodBuilder<TEmbedding> : ClassMethodBuilder<TEmbedding>
        where TEmbedding : class, new()
    {
        const string CompareMethodName = "Compare";
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassCompareMethodBuilder{TEmbedding}"/> class.
        /// </summary>
        /// <param name="embeddingId">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="logger">The logger.</param>
        public ClassCompareMethodBuilder(EmbeddingId embeddingId, IEventTypes eventTypes, ILogger logger)
            : base(embeddingId, eventTypes)
        {
            _logger = logger;
        }

        /// <summary>
        /// Try to build an <see cref="ICompareMethod{TEmbedding}"/>.
        /// </summary>
        /// <param name="method">The out of the method.</param>
        /// <returns>A bool indicating whether the build succeeded.</returns>
        public bool TryBuild(out ICompareMethod<TEmbedding> method)
        {
            var allMethods = EmbeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            if (!TryAddDecoratedCompareMethod(allMethods, out method)
                && !TryAddConventionCompareMethod(allMethods, out method))
            {
                _logger.LogWarning(
                    "No compare method defined for embedding {EmbeddingType} with id {EmbeddingId}. An embedding needs to have one compare method, which is either named {CompareName} or attributed with [{CompareAttribute}].",
                    EmbeddingType,
                    Embedding,
                    CompareMethodName,
                    nameof(CompareAttribute));
                return false;
            }

            return true;
        }

        bool TryAddDecoratedCompareMethod(
            IEnumerable<MethodInfo> allMethods,
            out ICompareMethod<TEmbedding> compareMethod)
        {
            compareMethod = default;
            if (allMethods.Count(IsDecoratedCompareMethod) > 1)
            {
                _logger.LogWarning(
                    "More than one Compare method attributed on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one Compare method.",
                    EmbeddingType,
                    Embedding);
                return false;
            }

            var decoratedMethod = allMethods.SingleOrDefault(IsDecoratedCompareMethod);
            if (!CompareMethodIsOkay(decoratedMethod))
            {
                return false;
            }

            compareMethod = CreateCompareMethod(decoratedMethod);
            return true;
        }

        bool TryAddConventionCompareMethod(
           IEnumerable<MethodInfo> allMethods,
           out ICompareMethod<TEmbedding> compareMethod)
        {
            compareMethod = default;

            if (allMethods.Count(_ => IsDecoratedCompareMethod(_) || _.Name == CompareMethodName) > 1)
            {
                _logger.LogWarning(
                    "More than one Compare method on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one Compare method called {CompareName} or attributed with [{CompareAttribute}}.",
                    EmbeddingType,
                    Embedding,
                    CompareMethodName,
                    nameof(CompareAttribute));
                return false;
            }

            var conventionMethod = allMethods
                .SingleOrDefault(_ => !IsDecoratedCompareMethod(_) && _.Name == CompareMethodName);
            if (!CompareMethodIsOkay(conventionMethod))
            {
                return false;
            }

            compareMethod = CreateCompareMethod(conventionMethod);
            return true;
        }

        bool CompareMethodIsOkay(MethodInfo method)
        {
            if (method == default)
            {
                return false;
            }

            if (!CompareMethodParametersAreOkay(method))
            {
                return false;
            }

            if (MethodReturnsTaskOrVoid(method))
            {
                _logger.LogWarning(
                    "Compare method {Method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.",
                    method,
                    EmbeddingType);
                return false;
            }
            return true;
        }

        bool CompareMethodParametersAreOkay(MethodInfo method)
        {
            var okay = true;
            if (!SecondMethodParameterIsEmbeddingContext(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Compare method {Method} on embedding {EmbeddingType} needs to have two parameters, where the second parameters is {EmbeddingContext}",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            if (!CompareMethodHasNoExtraParameters(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Compare method {Method} on embedding {EmbeddingType} needs to have two parameters, where the first one is the received state and the second is {EmbeddingContext}",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            return okay;
        }

        ICompareMethod<TEmbedding> CreateCompareMethod(MethodInfo method)
        {
            var compareSignatureType = GetCompareSignatureType(method);
            var compareSignature = method.CreateDelegate(compareSignatureType.MakeGenericType(EmbeddingType), null);
            return Activator.CreateInstance(
                typeof(ClassCompareMethod<>).MakeGenericType(EmbeddingType),
                compareSignature) as ICompareMethod<TEmbedding>;
        }

        Type GetCompareSignatureType(MethodInfo method)
        {
            if (MethodReturnsTaskOrVoid(method))
            {
                throw new InvalidCompareMethodReturnType(method.ReturnType);
            }

            if (MethodReturnsEnumerableObject(method))
            {
                return typeof(CompareMethodEnumerableReturnSignature<>);
            }

            return typeof(CompareMethodSignature<>);
        }

        bool SecondMethodParameterIsEmbeddingContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EmbeddingContext);

        bool IsDecoratedCompareMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(CompareAttribute), true).FirstOrDefault() != default;

        bool CompareMethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 2;
    }
}
