// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Exception that gets thrown when no read model is specified while creating an embedding.
/// </summary>
public class EmbeddingNeedsToBeForReadModel : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingNeedsToBeForReadModel"/> class.
    /// </summary>
    /// <param name="embeddingId">The <see cref="EmbeddingId" />.</param>
    public EmbeddingNeedsToBeForReadModel(EmbeddingId embeddingId)
        : base($"Embedding {embeddingId} could not be built because {nameof(EmbeddingBuilder.ForReadModel)} needs to be called when creating an embedding")
    {
    }
}