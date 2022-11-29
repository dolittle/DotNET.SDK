// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.Model;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Decorates a class to indicate the Embedding Id of the Embedding class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EmbeddingAttribute : Attribute, IDecoratedTypeDecorator<EmbeddingModelId>
{ 
    readonly EmbeddingId _id;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingAttribute"/> class.
    /// </summary>
    /// <param name="embeddingId">The unique identifier of the embedding.</param>
    public EmbeddingAttribute(string embeddingId)
    {
        _id = embeddingId;
    }

    /// <inheritdoc />
    public EmbeddingModelId GetIdentifier(Type decoratedType) => new(_id, decoratedType.Name);
}
