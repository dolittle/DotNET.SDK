// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Embeddings.Builder;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Internal;

/// <summary>
/// An implementation of <see cref="IEmbedding{TReadModel}" />.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public class Embedding<TReadModel> : IEmbedding<TReadModel>
    where TReadModel : class, new()
{
    readonly IEventTypes _eventTypes;
    readonly IDictionary<EventType, IOnMethod<TReadModel>> _onMethods;
    readonly IUpdateMethod<TReadModel> _updateMethod;
    readonly IDeleteMethod<TReadModel> _removeMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="Embedding{TReadModel}"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="EmbeddingId" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="onMethods">The on methods by <see cref="EventType" />.</param>
    /// <param name="updateMethod">The compare method.</param>
    /// <param name="removeMethod">The remove method.</param>
    public Embedding(
        EmbeddingId identifier,
        IEventTypes eventTypes,
        IDictionary<EventType, IOnMethod<TReadModel>> onMethods,
        IUpdateMethod<TReadModel> updateMethod,
        IDeleteMethod<TReadModel> removeMethod)
    {
        _onMethods = onMethods;
        _eventTypes = eventTypes;
        Identifier = identifier;
        Events = onMethods.Select(_ => _.Key);
        _updateMethod = updateMethod;
        _removeMethod = removeMethod;
        ReadModelType = typeof(TReadModel);
    }

    /// <inheritdoc />
    public Type ReadModelType { get; }

    /// <inheritdoc/>
    public EmbeddingId Identifier { get; }

    /// <inheritdoc/>
    public TReadModel InitialState { get; } = new TReadModel();

    /// <inheritdoc/>
    public IEnumerable<EventType> Events { get; }

    /// <inheritdoc/>
    public UncommittedEvents Update(TReadModel receivedState, TReadModel currentState, EmbeddingContext context, CancellationToken cancellation)
    {
        var tryUpdate = _updateMethod.TryUpdate(receivedState, currentState, context);
        if (tryUpdate.Exception != default)
        {
            throw new EmbeddingUpdateMethodFailed(Identifier, context, tryUpdate.Exception);
        }
        if (tryUpdate.Result.All(_ => _ == default))
        {
            throw new EmbeddingUpdateMethodDidNotReturnEvents(Identifier, context);
        }
        return CreateUncommittedEvents(tryUpdate.Result);
    }

    /// <inheritdoc/>
    public UncommittedEvents Delete(TReadModel currentState, EmbeddingContext context, CancellationToken cancellation)
    {
        var tryDelete = _removeMethod.TryDelete(currentState, context);
        if (tryDelete.Exception != default)
        {
            throw new EmbeddingDeleteMethodFailed(Identifier, context, tryDelete.Exception);
        }
        if (tryDelete.Result.All(_ => _ == default))
        {
            throw new EmbeddingDeleteMethodDidNotReturnEvents(Identifier, context);
        }
        return CreateUncommittedEvents(tryDelete.Result);
    }

    /// <inheritdoc/>
    public async Task<ProjectionResult<TReadModel>> On(TReadModel readModel, object @event, EventType eventType, EmbeddingProjectContext context, CancellationToken cancellation)
    {
        if (!_onMethods.TryGetValue(eventType, out var method))
        {
            throw new MissingOnMethodForEventType(eventType);
        }
        var tryOn = await method.TryOn(readModel, @event, context).ConfigureAwait(false);
        if (tryOn.Exception != default)
        {
            throw new EmbeddingOnMethodFailed(Identifier, eventType, @event, tryOn.Exception);
        }
        return tryOn.Result;
    }

    UncommittedEvents CreateUncommittedEvents(IEnumerable<object> events)
    {
        var result = new UncommittedEvents();
        foreach (var @event in events)
        {
            result.Add(new UncommittedEvent(Guid.Empty, _eventTypes.GetFor(@event.GetType()), @event, true));
        }

        return result;
    }
}
