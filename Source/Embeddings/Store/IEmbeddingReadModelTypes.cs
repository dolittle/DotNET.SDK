// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Defines a associations for embeddings.
/// </summary>
public interface IEmbeddingReadModelTypes : IUniqueBindings<EmbeddingId, Type>
{
    /// <summary>
    /// Try get the <see cref="EmbeddingId" /> associated with <typeparamref name="TEmbedding"/>.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>The <see cref="EmbeddingId" />.</returns>
    EmbeddingId GetFor<TEmbedding>()
        where TEmbedding : class, new();
}
