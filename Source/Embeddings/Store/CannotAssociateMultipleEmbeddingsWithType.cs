// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when attempting to associate multiple instance of embedding with a single <see cref="Type"/>.
/// </summary>
public class CannotAssociateMultipleEmbeddingsWithType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotAssociateMultipleEmbeddingsWithType"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that was attempted to associate with a <see cref="EmbeddingId"/>.</param>
    /// <param name="embedding">The <see cref="EmbeddingId"/> that was attempted to associate with.</param>
    /// <param name="existing">The <see cref="EmbeddingId"/> that the <see cref="Type"/> was already associated with.</param>
    public CannotAssociateMultipleEmbeddingsWithType(Type type, EmbeddingId embedding, EmbeddingId existing)
        : base($"{type} cannot be associated with {embedding} because it is already associated with {existing}")
    {
    }
}