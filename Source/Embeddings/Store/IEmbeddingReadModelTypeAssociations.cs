// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Store
{
    /// <summary>
    /// Defines a associations for embeddings.
    /// </summary>
    public interface IEmbeddingReadModelTypeAssociations
    {
        /// <summary>
        /// Associate an embedding.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId" />.</param>
        /// <param name="embeddingType">The <see cref="Type" /> of the embedding.</param>
        void Associate(EmbeddingId embedding, Type embeddingType);

        /// <summary>
        /// Associate an embedding.
        /// </summary>
        /// <typeparam name="TReadModel">The <see cref="Type" /> of the embedding.</typeparam>
        /// <param name="embedding">The <see cref="EmbeddingId" />.</param>
        void Associate<TReadModel>(EmbeddingId embedding)
            where TReadModel : class, new();

        /// <summary>
        /// Associate an embedding.
        /// </summary>
        /// <param name="embeddingType">The <see cref="Type" /> of the embedding.</param>
        void Associate(Type embeddingType);

        /// <summary>
        /// Associate an embedding.
        /// </summary>
        /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
        void Associate<TEmbedding>()
            where TEmbedding : class, new();

        /// <summary>
        /// Try get the <see cref="EmbeddingId" /> associated with <typeparamref name="TEmbedding"/>.
        /// </summary>
        /// <typeparam name="TEmbedding">The <see cref="Type" /> of the projection.</typeparam>
        /// <returns>The <see cref="EmbeddingId" />.</returns>
        EmbeddingId GetFor<TEmbedding>()
            where TEmbedding : class, new();
    }
}
