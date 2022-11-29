// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Represents a builder for building an embedding.
/// </summary>
public class EmbeddingBuilder : IEmbeddingBuilder, ICanTryBuildEmbedding
{
    readonly EmbeddingId _embeddingId;
    readonly IModelBuilder _modelBuilder;
    ICanTryBuildEmbedding _methodsBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingBuilder"/> class.
    /// </summary>
    /// <param name="embeddingId">The <see cref="EmbeddingId" />.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    public EmbeddingBuilder(EmbeddingId embeddingId, IModelBuilder modelBuilder)
    {
        _embeddingId = embeddingId;
        _modelBuilder = modelBuilder;
    }

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> ForReadModel<TReadModel>()
        where TReadModel : class, new()
    {
        if (_methodsBuilder != default)
        {
            throw new ReadModelAlreadyDefinedForEmbedding(_embeddingId, typeof(TReadModel));
        }

        _modelBuilder.BindIdentifierToType<EmbeddingModelId, EmbeddingId>(new EmbeddingModelId(_embeddingId, null), typeof(TReadModel));
        var builder = new EmbeddingBuilderForReadModel<TReadModel>(_embeddingId);
        _methodsBuilder = builder;
        return builder;
    }

    /// <inheritdoc/>
    public bool TryBuild(IEventTypes eventTypes, IClientBuildResults buildResults, out Internal.IEmbedding embedding)
    {
        embedding = default;
        if (_methodsBuilder != null)
        {
            return _methodsBuilder.TryBuild(eventTypes, buildResults, out embedding);
        }
        buildResults.AddFailure($"Failed to register embedding {_embeddingId}. No read model defined for embedding.");    
        return false;
    }

    /// <inheritdoc />
    public bool Equals(ICanTryBuildEmbedding other) => ReferenceEquals(this, other);
}
