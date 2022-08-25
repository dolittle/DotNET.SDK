// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Represents the identifier of an event handler in an application model.
/// </summary>
public class EmbeddingModelId : Identifier<EmbeddingId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingModelId"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EmbeddingId"/>.</param>
    public EmbeddingModelId(EmbeddingId id)
        : base("Embedding", id)
    {
    }
}
