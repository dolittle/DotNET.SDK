// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Internal
{
    /// <summary>
    /// Defines an embedding.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public interface IEmbedding<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Gets the unique identifier for embedding - <see cref="EmbeddingId" />.
        /// </summary>
        EmbeddingId Identifier { get; }

        /// <summary>
        /// Gets the initial <typeparamref name="TReadModel"/> read model state.
        /// </summary>
        TReadModel InitialState { get; }

        /// <summary>
        /// Gets the event types identified by its artifact that is handled by this embedding.
        /// </summary>
        IEnumerable<EventType> Events { get; }

        /// <summary>
        /// Handle an event and update a readmodel.
        /// </summary>
        /// <param name="readModel">The read model to update.</param>
        /// <param name="event">The event to handle.</param>
        /// <param name="eventType">The artifact representign the event type.</param>
        /// <param name="context">The context of the embedding projection.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="EmbeddingResult{TReadModel}"/>.</returns>
        Task<EmbeddingResult<TReadModel>> On(TReadModel readModel, object @event, EventType eventType, EmbeddingProjectContext context, CancellationToken cancellation);
    }
}
