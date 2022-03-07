// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Defines a builder for building a single embedding.
/// </summary>
public interface IEmbeddingBuilder
{
    /// <summary>
    /// Creates an embedding for the specified read model type.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
    /// <returns>The <see cref="IEmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    IEmbeddingBuilderForReadModel<TReadModel> ForReadModel<TReadModel>()
        where TReadModel : class, new();
}
