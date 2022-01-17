// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Store;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Defines an interface for working getting embeddings.
/// </summary>
public interface IEmbeddingStore
{
    /// <summary>
    /// Gets a embedding state by key for a embedding associated with a type.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the embedding.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TEmbedding}" />.</returns>
    Task<CurrentState<TEmbedding>> Get<TEmbedding>(Key key, CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Gets a embedding state by key for a embedding specified by embedding identifier.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the embedding.</param>
    /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TEmbedding}" />.</returns>
    Task<CurrentState<TEmbedding>> Get<TEmbedding>(Key key, EmbeddingId embeddingId, CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Gets a embedding state by key for a embedding specified by embedding identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the embedding.</param>
    /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{Object}" />.</returns>
    Task<CurrentState<object>> Get(Key key, EmbeddingId embeddingId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets all embedding states for a embedding associated with a type.
    /// </summary>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{TEmbedding}" />.</returns>
    Task<IDictionary<Key, CurrentState<TEmbedding>>> GetAll<TEmbedding>(CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Gets all embedding states for a embedding specified by embedding identifier.
    /// </summary>
    /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{TEmbedding}" />.</returns>
    Task<IDictionary<Key, CurrentState<TEmbedding>>> GetAll<TEmbedding>(EmbeddingId embeddingId, CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Gets all embedding states for a embedding specified by embedding identifier.
    /// </summary>
    /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{Object}" />.</returns>
    Task<IDictionary<Key, CurrentState<object>>> GetAll(EmbeddingId embeddingId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets all the keys for an embedding associated with a type.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{TResult}" /> of <see cref="Key" />.</returns>
    Task<IEnumerable<Key>> GetKeys<TEmbedding>(CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Gets all the keys for an embedding specified by embedding identifier.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{TResult}" /> of <see cref="Key" />.</returns>
    Task<IEnumerable<Key>> GetKeys<TEmbedding>(EmbeddingId embeddingId, CancellationToken cancellation = default)
        where TEmbedding : class, new();

    /// <summary>
    /// Gets all the keys for an embedding specified by embedding identifier.
    /// </summary>
    /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{TResult}" /> of <see cref="Key" />.</returns>
    Task<IEnumerable<Key>> GetKeys(EmbeddingId embeddingId, CancellationToken cancellation = default);
}