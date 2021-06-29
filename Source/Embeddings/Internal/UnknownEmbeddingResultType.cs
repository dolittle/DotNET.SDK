// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Internal
{
    /// <summary>
    /// Exception that gets thrown when trying to convert a <see cref="EmbeddingResultType"/> that does not have a known Contracts representation.
    /// </summary>
    public class UnknownEmbeddingResultType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownEmbeddingResultType"/> class.
        /// </summary>
        /// <param name="resultType">The <see cref="EmbeddingResultType"/> to be converted.</param>
        public UnknownEmbeddingResultType(EmbeddingResultType resultType)
            : base($"The embedding result type '{resultType}' is unknown.")
        {
        }
    }
}