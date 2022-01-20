// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Internal;

/// <summary>
/// Exception that gets thrown when an embeddings update method doesn't return any events.
/// </summary>
public class EmbeddingUpdateMethodDidNotReturnEvents : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingUpdateMethodDidNotReturnEvents"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId" />.</param>
    /// <param name="context">The <see cref="EmbeddingContext" />.</param>
    public EmbeddingUpdateMethodDidNotReturnEvents(EmbeddingId embedding, EmbeddingContext context)
        : base($"Embedding {embedding} didn't return any events from its update method for the key of: {context.Key}")
    {
    }
}