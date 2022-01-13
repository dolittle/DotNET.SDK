// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Represents an implementation of <see cref="IEmbeddingReadModelTypes" />.
/// </summary>
public class EmbeddingReadModelTypes : UniqueBindings<EmbeddingId, Type>, IEmbeddingReadModelTypes
{
    /// <inheritdoc/>
    public EmbeddingId GetFor<TEmbedding>()
        where TEmbedding : class, new()
        => base.GetFor(typeof(TEmbedding));
}
