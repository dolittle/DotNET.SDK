// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when attempting to associate multiple instance of <see cref="Type"/> with a single embedding.
/// </summary>
public class CannotAssociateMultipleTypesWithEmbedding : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotAssociateMultipleTypesWithEmbedding"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/> that was attempted to associate with a <see cref="Type"/>.</param>
    /// <param name="type">The <see cref="Type"/> that was attempted to associate with.</param>
    /// <param name="existing">The <see cref="Type"/> that the <see cref="EmbeddingId"/> was already associated with.</param>
    public CannotAssociateMultipleTypesWithEmbedding(EmbeddingId embedding, Type type, Type existing)
        : base($"{embedding} cannot be associated with {type} because it is already associated with {existing}")
    {
    }
}