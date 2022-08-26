// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Embeddings.Internal;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Methods for building <see cref="IEmbedding{TReadModel}"/> instances by convention from an instantiated embedding class.
/// </summary>
/// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
public class ConventionEmbeddingBuilder<TEmbedding> : ICanTryBuildEmbedding
    where TEmbedding : class, new()
{
    readonly Type _embeddingType = typeof(TEmbedding);

    /// <inheritdoc/>
    public bool TryBuild(EmbeddingModelId embeddingId, IEventTypes eventTypes, IClientBuildResults buildResults, out Internal.IEmbedding embedding)
    {
        embedding = default;
        buildResults.AddInformation($"Building embedding {embeddingId.Id} from type {_embeddingType}");

        if (!HasParameterlessConstructor())
        {
            buildResults.AddFailure($"The embedding class {_embeddingType} has no default/parameterless constructor");
            return false;
        }

        if (HasMoreThanOneConstructor())
        {
            buildResults.AddFailure($"The embedding class {_embeddingType} has more than one constructor. It must only have one, parameterless, constructor");
            return false;
        }

        var success = ClassMethodBuilder<TEmbedding>
            .ForProjection(embeddingId.Id, eventTypes, buildResults)
            .TryBuild(out var eventTypesToMethods);

        success = ClassMethodBuilder<TEmbedding>
            .ForUpdate(embeddingId.Id, eventTypes, buildResults)
            .TryBuild(out var updateMethod) && success;


        success = ClassMethodBuilder<TEmbedding>
            .ForDelete(embeddingId.Id, eventTypes, buildResults)
            .TryBuild(out var deleteMethod) && success;

        if (!success)
        {
            return false;
        }
            
        embedding = new Embedding<TEmbedding>(
            embeddingId,
            eventTypes,
            eventTypesToMethods,
            updateMethod,
            deleteMethod);

        return true;
    }


    bool HasMoreThanOneConstructor()
        => _embeddingType.GetConstructors().Length > 1;

    bool HasParameterlessConstructor()
        => _embeddingType.GetConstructors().Any(t => t.GetParameters().Length == 0);

    /// <inheritdoc />
    public bool Equals(IProcessorBuilder<EmbeddingModelId, EmbeddingId> other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other is ConventionEmbeddingBuilder<TEmbedding> otherBuilder
            && _embeddingType == otherBuilder._embeddingType;
    }
}
