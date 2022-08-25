// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

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
    protected Type EmbeddingType { get; }

    /// <summary>
    /// Creates a new <see cref="ClassUpdateMethodBuilder{TEmbedding}"/>.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    /// <param name="eventTypes">The event types.<see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <returns>The <see cref="ClassUpdateMethodBuilder{TEmbedding}"/>.</returns>
    public static ClassUpdateMethodBuilder<TEmbedding> ForUpdate(EmbeddingId embedding, IEventTypes eventTypes, IClientBuildResults buildResults)
        => new(embedding, eventTypes, buildResults);

    /// <summary>
    /// Creates a new <see cref="ClassDeleteMethodBuilder{TEmbedding}"/>.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    /// <param name="eventTypes">The event types.<see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <returns>The <see cref="ClassDeleteMethodBuilder{TEmbedding}"/>.</returns>
    public static ClassDeleteMethodBuilder<TEmbedding> ForDelete(EmbeddingId embedding, IEventTypes eventTypes, IClientBuildResults buildResults)
        => new(embedding, eventTypes, buildResults);

    /// <summary>
    /// Creates a new <see cref="ClassProjectionMethodsBuilder{TEmbedding}"/>.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    /// <param name="eventTypes">The event types.<see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <returns>The <see cref="ClassProjectionMethodsBuilder{TEmbedding}"/>.</returns>
    public static ClassProjectionMethodsBuilder<TEmbedding> ForProjection(EmbeddingId embedding, IEventTypes eventTypes, IClientBuildResults buildResults)
        => new(embedding, eventTypes, buildResults);

    /// <summary>
    /// Whether the method returns an enumerable object.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns>Whether the method returns an enumerable.</returns>
    protected bool MethodReturnsEnumerableObject(MethodInfo method)
        => typeof(System.Collections.IEnumerable).IsAssignableFrom(method.ReturnType);

    /// <summary>
    /// Whether the method returns a Task.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns>Whether the method returns a <see cref="Task" />.</returns>
    protected bool MethodReturnsTask(MethodInfo method)
        => typeof(Task).IsAssignableFrom(method.ReturnType);

    /// <summary>
    /// Whether the method returns a void.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns>Whether the method returns an void.</returns>
    protected bool MethodReturnsVoid(MethodInfo method)
        => method.ReturnType == typeof(void);

    /// <summary>
    /// Whether the method returns an async void.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns>Whether the method returns async void.</returns>
    protected bool MethodReturnsAsyncVoid(MethodInfo method)
    {
        var asyncAttribute = typeof(AsyncStateMachineAttribute);
        var isAsyncMethod = (AsyncStateMachineAttribute)method.GetCustomAttribute(asyncAttribute) != null;
        return isAsyncMethod && MethodReturnsVoid(method);
    }

    /// <summary>
    /// Whether the method returns a task or void.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns>Whether the method returns task or void.</returns>
    protected bool MethodReturnsTaskOrVoid(MethodInfo method) =>
        MethodReturnsTask(method)
        || MethodReturnsVoid(method)
        || MethodReturnsAsyncVoid(method);
}
