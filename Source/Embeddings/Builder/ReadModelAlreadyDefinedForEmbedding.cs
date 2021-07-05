// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Exception that gets thrown when trying to define another readmodel for an embedding.
    /// </summary>
    public class ReadModelAlreadyDefinedForEmbedding : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadModelAlreadyDefinedForEmbedding"/> class.
        /// </summary>
        /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
        /// <param name="type">The type of the readmodel already defined.</param>
        public ReadModelAlreadyDefinedForEmbedding(EmbeddingId embeddingId, Type type)
            : base($"Embedding {embeddingId} already has a readmodel of type {type} defined for it.")
        {
        }
    }
}
