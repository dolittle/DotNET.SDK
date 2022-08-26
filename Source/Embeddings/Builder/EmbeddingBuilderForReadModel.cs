// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Embeddings.Internal;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Represents a builder for building an embeddings methods.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
public class EmbeddingBuilderForReadModel<TReadModel> : ICanTryBuildEmbedding, IEmbeddingBuilderForReadModel<TReadModel>
    where TReadModel : class, new()
{
    readonly List<IOnMethod<TReadModel>> _methods = new();
    readonly EmbeddingModelId _embeddingId;
    IUpdateMethod<TReadModel>? _updateMethod;
    IDeleteMethod<TReadModel>? _deleteMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingBuilderForReadModel{TReadModel}"/> class.
    /// </summary>
    /// <param name="embeddingId">The <see cref="EmbeddingId" />.</param>
    public EmbeddingBuilderForReadModel(EmbeddingModelId embeddingId)
    {
        _embeddingId = embeddingId;
    }
    
    /// <inheritdoc />
    public bool Equals(IProcessorBuilder<EmbeddingModelId, EmbeddingId> other) => ReferenceEquals(this, other);

    /// <summary>
    /// Add the update method for resolving the received and current state of the embedding.
    /// The method should return a single event, that changes the current state more towards the received state.
    /// This method will be called until the received state and current state are equal.
    /// </summary>
    /// <param name="method">The <see cref="UpdateSignature{TReadModel}"/>.</param>
    /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    public IEmbeddingBuilderForReadModel<TReadModel> ResolveUpdateToEvents(UpdateSignature<TReadModel> method)
    {
        ThrowIfUpdateMethodSet();
        _updateMethod = new UpdateMethod<TReadModel>(method);
        return this;
    }

    /// <summary>
    /// Add the update method for resolving the received and current state of the embedding.
    /// The method should return an enumerable of  events, that change the current state more towards the received state.
    /// This method will be called until the received state and current state are equal.
    /// </summary>
    /// <param name="method">The <see cref="UpdateEnumerableReturnSignature{TReadModel}"/>.</param>
    /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    public IEmbeddingBuilderForReadModel<TReadModel> ResolveUpdateToEvents(UpdateEnumerableReturnSignature<TReadModel> method)
    {
        ThrowIfUpdateMethodSet();
        _updateMethod = new UpdateMethod<TReadModel>(method);
        return this;
    }

    /// <summary>
    /// Add the delete method for resolving the events needed to delete the embedding.
    /// </summary>
    /// <param name="method">The <see cref="DeleteSignature{TReadModel}"/>.</param>
    /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    public IEmbeddingBuilderForReadModel<TReadModel> ResolveDeletionToEvents(DeleteSignature<TReadModel> method)
    {
        ThrowIfDeleteMethodSet();
        _deleteMethod = new DeleteMethod<TReadModel>(method);
        return this;
    }

    /// <summary>
    /// Add the delete method for resolving the events needed to delete the embedding.
    /// </summary>
    /// <param name="method">The <see cref="DeleteSignature{TReadModel}"/>.</param>
    /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
    public IEmbeddingBuilderForReadModel<TReadModel> ResolveDeletionToEvents(DeleteEnumerableReturnSignature<TReadModel> method)
    {
        ThrowIfDeleteMethodSet();
        _deleteMethod = new DeleteMethod<TReadModel>(method);
        return this;
    }

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On<TEvent>(TaskOnSignature<TReadModel, TEvent> method)
        where TEvent : class
    {
        _methods.Add(new TypedOnMethod<TReadModel, TEvent>(method));
        return this;
    }

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On<TEvent>(SyncOnSignature<TReadModel, TEvent> method)
        where TEvent : class
    {
        _methods.Add(new TypedOnMethod<TReadModel, TEvent>(method));
        return this;
    }

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On(EventType eventType, TaskOnSignature<TReadModel> method)
    {
        _methods.Add(new OnMethod<TReadModel>(method, eventType));
        return this;
    }

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On(EventType eventType, SyncOnSignature<TReadModel> method)
    {
        _methods.Add(new OnMethod<TReadModel>(method, eventType));
        return this;
    }

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, TaskOnSignature<TReadModel> method)
        => On(new EventType(eventTypeId), method);

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, SyncOnSignature<TReadModel> method)
        => On(new EventType(eventTypeId), method);

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, TaskOnSignature<TReadModel> method)
        => On(new EventType(eventTypeId, eventTypeGeneration), method);

    /// <inheritdoc />
    public IEmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, SyncOnSignature<TReadModel> method)
        => On(new EventType(eventTypeId, eventTypeGeneration), method);

    /// <inheritdoc/>
    public bool TryBuild(EmbeddingModelId embeddingId, IEventTypes eventTypes, IClientBuildResults buildResults, out Internal.IEmbedding embedding)
    {
        embedding = default;
        var eventTypesToMethods = new Dictionary<EventType, IOnMethod<TReadModel>>();
        if (!TryAddOnMethods(eventTypes, eventTypesToMethods, buildResults))
        {
            buildResults.AddFailure($"Failed to build embedding {_embeddingId}. One or more on methods could not be built");
            return false;
        }

        if (eventTypesToMethods.Count < 1)
        {
            buildResults.AddFailure($"Failed to build embedding {_embeddingId}. No on methods are configured for embedding");
            return false;
        }

        embedding = new Embedding<TReadModel>(
            _embeddingId,
            eventTypes,
            eventTypesToMethods,
            _updateMethod,
            _deleteMethod);
        return true;
    }

    bool TryAddOnMethods(IEventTypes eventTypes, IDictionary<EventType, IOnMethod<TReadModel>> eventTypesToMethods, IClientBuildResults buildResults)
    {
        var okay = true;
        foreach (var method in _methods)
        {
            var eventType = method.GetEventType(eventTypes);
            if (eventTypesToMethods.TryAdd(eventType, method))
            {
                continue;
            }
            okay = false;
            buildResults.AddFailure($"Embedding {_embeddingId} already handles event with event type {eventType}");
        }

        return okay;
    }

    void ThrowIfUpdateMethodSet()
    {
        if (_updateMethod != default)
        {
            throw new EmbeddingAlreadyHasAnUpdateMethod(_embeddingId);
        }
    }

    void ThrowIfDeleteMethodSet()
    {
        if (_deleteMethod != default)
        {
            throw new EmbeddingAlreadyHasADeleteMethod(_embeddingId);
        }
    }
}
