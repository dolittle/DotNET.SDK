// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Store;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Defines an embedding.
/// </summary>
public interface IEmbedding : IEmbeddingStore
{
    /// <summary>
    /// Updates an embedding state by key by calling the compare method for the embedding associated with a type.
    /// It will keep calling the compare method to commit events until the embedding reaches the desired state.
    /// </summary>
    /// <param name="key">The key of the embedding.</param>
    /// <param name="state">The new desired state of the embedding.</param>
    /// <param name="cancellation">The cancellation token. </param>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="CurrentState{TEmbedding}" />.</returns>
    Task<CurrentState<TEmbedding>> Update<TEmbedding>(Key key, TEmbedding state, CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Updates an embedding state by key by calling the compare method for the embedding associated with embedding identifier.
    /// It will keep calling the compare method to commit events until the embedding reaches the desired state.
    /// </summary>
    /// <param name="key">The key of the embedding.</param>
    /// <param name="embeddingId">The embedding identifier.</param>
    /// <param name="state">The new desired state of the embedding.</param>
    /// <param name="cancellation">The cancellation token. </param>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="CurrentState{TEmbedding}" />.</returns>
    Task<CurrentState<TEmbedding>> Update<TEmbedding>(Key key, EmbeddingId embeddingId, TEmbedding state, CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Updates an embedding state by key by calling the compare method for the embedding associated with embedding identifier.
    /// It will keep calling the compare method to commit events until the embedding reaches the desired state.
    /// </summary>
    /// <param name="key">The key of the embedding.</param>
    /// <param name="embeddingId">The embedding identifier.</param>
    /// <param name="state">The new desired state of the embedding.</param>
    /// <param name="cancellation">The cancellation token. </param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="CurrentState{TEmbedding}" />.</returns>
    Task<CurrentState<object>> Update(Key key, EmbeddingId embeddingId, object state, CancellationToken cancellation = default);

    /// <summary>
    /// Deletes an embedding state by key for the embedding associated with a type.
    /// It will keep calling the remove method to commit events until the embedding is deleted.
    /// </summary>
    /// <param name="key">The key of the embedding.</param>
    /// <param name="cancellation">The cancellation token. </param>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
    Task Delete<TEmbedding>(Key key, CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Deletes an embedding state by key for the embedding associated with a type.
    /// It will keep calling the remove method to commit events until the embedding is deleted.
    /// </summary>
    /// <param name="key">The key of the embedding.</param>
    /// <param name="embeddingId">The embedding identifier.</param>
    /// <param name="cancellation">The cancellation token. </param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
    Task Delete(Key key, EmbeddingId embeddingId, CancellationToken cancellation = default);
}