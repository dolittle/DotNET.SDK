// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Exception that gets thrown when trying to define another remove method for an embedding.
    /// </summary>
    public class EmbeddingAlreadyHasARemoveMethod : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingAlreadyHasARemoveMethod"/> class.
        /// </summary>
        /// <param name="embeddingId">The <see cref="EmbeddingId"/>.</param>
        public EmbeddingAlreadyHasARemoveMethod(EmbeddingId embeddingId)
            : base($"Embedding {embeddingId} already has a remove method defined for it.")
        {
        }
    }
}
