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
    /// Builder for building <see cref="ClassDeleteMethod{TEmbedding}"/>.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    public class ClassDeleteMethodBuilder<TEmbedding> : ClassMethodBuilder<TEmbedding>
        where TEmbedding : class, new()
    {
        const string DeleteMethodName = "ResolveDeletionToEvents";
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDeleteMethodBuilder{TEmbedding}"/> class.
        /// </summary>
        /// <param name="embeddingId">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="logger">The logger.</param>
        public ClassDeleteMethodBuilder(EmbeddingId embeddingId, IEventTypes eventTypes, ILogger logger)
            : base(embeddingId, eventTypes)
        {
            _logger = logger;
        }

        /// <summary>
        /// Try to build an <see cref="IDeleteMethod{TEmbedding}"/>.
        /// </summary>
        /// <param name="method">The out of the method.</param>
        /// <returns>A bool indicating whether the build succeeded.</returns>
        public bool TryBuild(out IDeleteMethod<TEmbedding> method)
        {
            var allMethods = EmbeddingType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            if (!TryAddDecoratedDeleteMethod(allMethods, out method)
                && !TryAddConventionDeleteMethod(allMethods, out method))
            {
                _logger.LogWarning(
                    "No deletion method defined for embedding {EmbeddingType} with id {EmbeddingId}. An embedding needs to have one deletion method, which is either named {DeleteName} or attributed with [{DeleteAttribute}].",
                    EmbeddingType,
                    Embedding,
                    DeleteMethodName,
                    nameof(ResolveDeletionToEventsAttribute));
                return false;
            }

            return true;
        }

        bool TryAddDecoratedDeleteMethod(
            IEnumerable<MethodInfo> allMethods,
            out IDeleteMethod<TEmbedding> deleteMethod)
        {
            deleteMethod = default;
            if (allMethods.Count(IsDecoratedDeleteMethod) > 1)
            {
                _logger.LogWarning(
                    "More than one Delete method attributed on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one Delete method.",
                    EmbeddingType,
                    Embedding);
                return false;
            }

            var decoratedMethod = allMethods.SingleOrDefault(IsDecoratedDeleteMethod);
            if (!DeleteMethodIsOkay(decoratedMethod))
            {
                return false;
            }

            deleteMethod = CreateDeleteMethod(decoratedMethod);
            return true;
        }

        bool TryAddConventionDeleteMethod(
            IEnumerable<MethodInfo> allMethods,
            out IDeleteMethod<TEmbedding> deleteMethod)
        {
            deleteMethod = default;

            if (allMethods.Count(_ => IsDecoratedDeleteMethod(_) || _.Name == DeleteMethodName) > 1)
            {
                _logger.LogWarning(
                    "More than one deletion method on embedding {EmbeddingType} with id {EmbeddingId}. An embedding can only have one deletion method called {DeletionName} or attributed with [{DeletionAttribute}}.",
                    EmbeddingType,
                    Embedding,
                    DeleteMethodName,
                    nameof(ResolveDeletionToEventsAttribute));
                return false;
            }

            var conventionMethod = allMethods
                .SingleOrDefault(_ => !IsDecoratedDeleteMethod(_) && _.Name == DeleteMethodName);
            if (!DeleteMethodIsOkay(conventionMethod))
            {
                return false;
            }

            deleteMethod = CreateDeleteMethod(conventionMethod);
            return true;
        }

        bool DeleteMethodIsOkay(MethodInfo method)
        {
            if (method == default)
            {
                return false;
            }

            if (!DeleteMethodParametersAreOkay(method))
            {
                return false;
            }

            if (MethodReturnsTaskOrVoid(method))
            {
                _logger.LogWarning(
                    "Deletion method {Method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.",
                    method,
                    EmbeddingType);
                return false;
            }

            return true;
        }

        bool DeleteMethodParametersAreOkay(MethodInfo method)
        {
            var okay = true;
            if (!FirstMethodParameterIsEmbeddingContext(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Deletion method {Method} on embedding {EmbeddingType} needs to have only one {EmbeddingContext} parameter",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            if (!DeleteMethodHasNoExtraParameters(method))
            {
                okay = false;
                _logger.LogWarning(
                    "Deletion method {Method} on embedding {EmbeddingType} needs to have two parameters, where the first one is the received state and the second is {EmbeddingContext}",
                    method,
                    EmbeddingType,
                    typeof(EmbeddingContext));
            }

            if (MethodReturnsTaskOrVoid(method))
            {
                _logger.LogWarning(
                    "Deletion method {Method} on embedding {EmbeddingType} needs to return either an object or an IEnumerable<object>.",
                    method,
                    EmbeddingType);
                return false;
            }

            return okay;
        }

        IDeleteMethod<TEmbedding> CreateDeleteMethod(MethodInfo method)
        {
            var deleteSignatureType = GetDeleteSignatureType(method);
            var deleteSignature = method.CreateDelegate(deleteSignatureType.MakeGenericType(EmbeddingType), null);
            return Activator.CreateInstance(
                typeof(ClassDeleteMethod<>).MakeGenericType(EmbeddingType),
                deleteSignature) as IDeleteMethod<TEmbedding>;
        }

        Type GetDeleteSignatureType(MethodInfo method)
        {
            if (MethodReturnsTaskOrVoid(method))
            {
                throw new InvalidDeleteMethodReturnType(method.ReturnType);
            }

            if (MethodReturnsEnumerableObject(method))
            {
                return typeof(DeleteMethodEnumerableReturnSignature<>);
            }

            return typeof(DeleteMethodSignature<>);
        }

        bool FirstMethodParameterIsEmbeddingContext(MethodInfo method)
            => method.GetParameters().Length > 0 && method.GetParameters()[0].ParameterType == typeof(EmbeddingContext);

        bool IsDecoratedDeleteMethod(MethodInfo method)
            => method.GetCustomAttributes(typeof(ResolveDeletionToEventsAttribute), true).FirstOrDefault() != default;

        bool DeleteMethodHasNoExtraParameters(MethodInfo method)
            => method.GetParameters().Length == 1;
    }
}
