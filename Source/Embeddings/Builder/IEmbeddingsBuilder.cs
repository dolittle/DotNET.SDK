// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Defines a builder for building embeddings.
/// </summary>
public interface IEmbeddingsBuilder
{
    /// <summary>
    /// Start building an embedding.
    /// </summary>
    /// <param name="embeddingId">The <see cref="EmbeddingId" />.</param>
    /// <returns>The <see cref="IEmbeddingBuilder" /> for continuation.</returns>
    IEmbeddingBuilder Create(EmbeddingId embeddingId);

    /// <summary>
    /// Registers a <see cref="Type" /> as an embedding class.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the embedding class.</typeparam>
    /// <returns>The <see cref="IEmbeddingsBuilder" /> for continuation.</returns>
    public IEmbeddingsBuilder Register<TProjection>()
        where TProjection : class, new();

    /// <summary>
    /// Registers a <see cref="Type" /> as an embedding class.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> of the embedding.</param>
    /// <returns>The <see cref="IEmbeddingsBuilder" /> for continuation.</returns>
    public IEmbeddingsBuilder Register(Type type);

    /// <summary>
    /// Registers all embedding classes from an <see cref="Assembly" />.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly" /> to register the embedding classes from.</param>
    /// <returns>The <see cref="IEmbeddingsBuilder" /> for continuation.</returns>
    public IEmbeddingsBuilder RegisterAllFrom(Assembly assembly);
}
