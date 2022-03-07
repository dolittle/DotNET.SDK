// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Represents the signature for an inline embeddings delete method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="currentState">The current state.</param>
/// <param name="context">The <see cref="EmbeddingContext"/>.</param>
/// <returns>One event.</returns>
public delegate object DeleteSignature<TReadModel>(
    TReadModel currentState,
    EmbeddingContext context);

/// <summary>
/// Represents the signature for an inline embeddings deletion method.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
/// <param name="currentState">The current state.</param>
/// <param name="context">The <see cref="EmbeddingContext"/>.</param>
/// <returns>An <see cref="IEnumerable{TResult}"/> of events..</returns>
public delegate IEnumerable<object> DeleteEnumerableReturnSignature<TReadModel>(
    TReadModel currentState,
    EmbeddingContext context);

/// <summary>
/// Represents the signature for an embedding class's deletion method.
/// </summary>
/// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
/// <param name="instance">The current state.</param>
/// <param name="context">The <see cref="EmbeddingContext"/>.</param>
/// <returns>One event.</returns>
public delegate object DeleteMethodSignature<TEmbedding>(
    TEmbedding instance,
    EmbeddingContext context);

/// <summary>
/// Represents the signature for an embedding class's deletion method.
/// </summary>
/// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
/// <param name="instance">The current state.</param>
/// <param name="context">The <see cref="EmbeddingContext"/>.</param>
/// <returns>An <see cref="IEnumerable{TResult}"/> of events.</returns>
public delegate IEnumerable<object> DeleteMethodEnumerableReturnSignature<TEmbedding>(
    TEmbedding instance,
    EmbeddingContext context);