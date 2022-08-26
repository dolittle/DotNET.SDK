// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Decorates a class to indicate the Embedding Id of the Embedding class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EmbeddingAttribute : Attribute, IDecoratedTypeDecorator<EmbeddingModelId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingAttribute"/> class.
    /// </summary>
    /// <param name="embeddingId">The unique identifier of the embedding.</param>
    public EmbeddingAttribute(string embeddingId)
    {
        Identifier = Guid.Parse(embeddingId);
    }

    /// <summary>
    /// Gets the unique identifier for this embedding.
    /// </summary>
    public EmbeddingId Identifier { get; }

    /// <inheritdoc />
    public EmbeddingModelId GetIdentifier() => new(Identifier, "");
}
