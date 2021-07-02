// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Defines the base class for a builder that can build class methods.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    public abstract class ClassMethodBuilder<TEmbedding>
        where TEmbedding : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMethodBuilder{TEmbedding}"/> class.
        /// </summary>
        /// <param name="embeddingId">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        protected ClassMethodBuilder(EmbeddingId embeddingId, IEventTypes eventTypes)
        {
            Embedding = embeddingId;
            EventTypes = eventTypes;
            EmbeddingType = typeof(TEmbedding);
        }

        /// <summary>
        /// Gets the <see cref="EmbeddingId" />.
        /// </summary>
        protected EmbeddingId Embedding { get; }

        /// <summary>
        /// Gets the <see cref="IEventTypes" />.
        /// </summary>
        protected IEventTypes EventTypes { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the embedding.
        /// </summary>
        protected Type EmbeddingType { get;  }

        /// <summary>
        /// Whether the method returns an enumerable object.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A bool.</returns>
        protected bool MethodReturnsEnumerableObject(MethodInfo method)
            => typeof(System.Collections.IEnumerable).IsAssignableFrom(method.ReturnType);

        /// <summary>
        /// Whether the method returns an object.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A bool.
        protected bool MethodReturnsObject(MethodInfo method)
            => typeof(object).IsAssignableFrom(method.ReturnType)
                && !MethodReturnsTask(method)
                && !method.ReturnType.IsGenericType;

        /// <summary>
        /// Whether the method returns a Task.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A bool.
        protected bool MethodReturnsTask(MethodInfo method)
            => method.ReturnType == typeof(Task);

        /// <summary>
        /// Whether the method returns a void.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A bool.
        protected bool MethodReturnsVoid(MethodInfo method)
            => method.ReturnType == typeof(void);

        /// <summary>
        /// Whether the method returns an async void.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A bool.
        protected bool MethodReturnsAsyncVoid(MethodInfo method)
        {
            var asyncAttribute = typeof(AsyncStateMachineAttribute);
            var isAsyncMethod = (AsyncStateMachineAttribute)method.GetCustomAttribute(asyncAttribute) != null;
            return isAsyncMethod && MethodReturnsVoid(method);
        }

        /// <summary>
        /// Whether the methods second parameter is an embedding context.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A bool.
        protected bool SecondMethodParameterIsEmbeddingContext(MethodInfo method)
            => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EmbeddingContext);

        /// <summary>
        /// Creates a new <see cref="ClassCompareMethodBuilder{TEmbedding}"/>.
        /// </summary>
        /// <param name="embedding">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="loggerFactory">The logger factory to create the logger from.</param>
        /// <returns>The <see cref="ClassCompareMethodBuilder{TEmbedding}"/></returns>
        public static ClassCompareMethodBuilder<TEmbedding> ForCompare(EmbeddingId embedding, IEventTypes eventTypes, ILoggerFactory loggerFactory)
            => new ClassCompareMethodBuilder<TEmbedding>(
                embedding,
                eventTypes,
                loggerFactory.CreateLogger<ClassCompareMethodBuilder<TEmbedding>>());

        /// <summary>
        /// Creates a new <see cref="ClassRemoveMethodBuilder{TEmbedding}"/>.
        /// </summary>
        /// <param name="embedding">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="loggerFactory">The logger factory to create the logger from.</param>
        /// <returns>The <see cref="ClassRemoveMethodBuilder{TEmbedding}"/></returns>
        public static ClassRemoveMethodBuilder<TEmbedding> ForRemove(EmbeddingId embedding, IEventTypes eventTypes, ILoggerFactory loggerFactory)
            => new ClassRemoveMethodBuilder<TEmbedding>(
                embedding,
                eventTypes,
                loggerFactory.CreateLogger<ClassRemoveMethodBuilder<TEmbedding>>());

        /// <summary>
        /// Creates a new <see cref="ClassProjectionMethodsBuilder{TEmbedding}"/>.
        /// </summary>
        /// <param name="embedding">The embedding identifier.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="loggerFactory">The logger factory to create the logger from.</param>
        /// <returns>The <see cref="ClassProjectionMethodsBuilder{TEmbedding}"/></returns>
        public static ClassProjectionMethodsBuilder<TEmbedding> ForProjection(EmbeddingId embedding, IEventTypes eventTypes, ILoggerFactory loggerFactory)
            => new ClassProjectionMethodsBuilder<TEmbedding>(
                embedding,
                eventTypes,
                loggerFactory.CreateLogger<ClassProjectionMethodsBuilder<TEmbedding>>());
    }
}
