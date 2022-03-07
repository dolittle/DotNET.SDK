// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when a type has no embedding associated with it.
/// </summary>
public class NoEmbeddingAssociatedWithType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoEmbeddingAssociatedWithType"/> class.
    /// </summary>
    /// <param name="type">The type without a embedding.</param>
    public NoEmbeddingAssociatedWithType(Type type)
        : base($"No embedding associated with type {type}")
    {
    }
}