// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Exception that gets thrown when trying to define another update method for an embedding.
    /// </summary>
    public class EmbeddingAlreadyHasAnUpdateMethod : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingAlreadyHasAnUpdateMethod"/> class.
        /// </summary>
        /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
        public EmbeddingAlreadyHasAnUpdateMethod(EmbeddingId embeddingId)
            : base($"Embedding {embeddingId} already has an update method defined for it.")
        {
        }
    }
}
