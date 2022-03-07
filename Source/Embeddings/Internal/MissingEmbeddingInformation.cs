// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Internal;

/// <summary>
/// Exception that gets thrown when there are details missing on an embedding.
/// </summary>
public class MissingEmbeddingInformation : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingEmbeddingInformation"/> class.
    /// </summary>
    /// <param name="details">The details that are missing.</param>
    public MissingEmbeddingInformation(string details)
        : base($"Missing information on embedding: {details}")
    {
    }
}