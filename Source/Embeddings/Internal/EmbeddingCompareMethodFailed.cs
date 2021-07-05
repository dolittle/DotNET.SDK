// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Internal
{
    /// <summary>
    /// Exception that gets thrown when an embedding compare-method comparing states.
    /// </summary>
    public class EmbeddingCompareMethodFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingCompareMethodFailed"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId" />.</param>
        /// <param name="context">The <see cref="EmbeddingContext" />.</param>
        /// <param name="exception">The <see cref="Exception" /> that caused the handling to fail.</param>
        public EmbeddingCompareMethodFailed(EmbeddingId embedding, EmbeddingContext context, Exception exception)
            : base($"Embedding {embedding} failed to compare current state with received state for key {context.Key}", exception)
        {
        }
    }
}
