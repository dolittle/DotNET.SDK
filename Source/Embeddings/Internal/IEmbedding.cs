// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Projections;

namespace Dolittle.SDK.Embeddings.Internal;

/// <summary>
/// Defines an embedding.
/// </summary>
public interface IEmbedding
{
    /// <summary>
    /// Gets the <see cref="Type"/> of the read model.
    /// </summary>
    Type ReadModelType { get; }
    
    /// <summary>
    /// Gets the unique identifier for embedding - <see cref="EmbeddingId" />.
    /// </summary>
    EmbeddingModelId Identifier { get; }

    /// <summary>
    /// Gets the event types identified by its artifact that is handled by this embedding.
    /// </summary>
    IEnumerable<EventType> Events { get; }
}

/// <summary>
/// Defines an embedding.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public interface IEmbedding<TReadModel> : IEmbedding
    where TReadModel : class, new()
{
    /// <summary>
    /// Gets the initial <typeparamref name="TReadModel"/> read model state.
    /// </summary>
    TReadModel InitialState { get; }
    
    /// <summary>
    /// Handle an event and update the read model.
    /// </summary>
    /// <param name="readModel">The read model to update.</param>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventType">The artifact representing the event type.</param>
    /// <param name="context">The context of the embedding projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="ProjectionResult{TReadModel}"/>.</returns>
    Task<ProjectionResult<TReadModel>> On(TReadModel readModel, object @event, EventType eventType, EmbeddingProjectContext context, CancellationToken cancellation);

    /// <summary>
    /// Called, when the read model should be updated.
    /// Returns events that should result in the current state matching the received state.
    /// </summary>
    /// <param name="receivedState">The received state.</param>
    /// <param name="currentState">The current state.</param>
    /// <param name="context">The context of the embedding.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="UncommittedEvents"/> to commit.</returns>
    UncommittedEvents Update(TReadModel receivedState, TReadModel currentState, EmbeddingContext context, CancellationToken cancellation);

    /// <summary>
    /// Called, when the read model should get deleted. Returns events that should result in the read model deletion.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <param name="context">The context of the embedding.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="UncommittedEvents"/> to commit.</returns>
    UncommittedEvents Delete(TReadModel currentState, EmbeddingContext context, CancellationToken cancellation);
}
