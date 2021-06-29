// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Embeddings.Internal;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Defines an embedding on-method.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public interface IEmbeddingOnMethod<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Invokes the on method.
        /// </summary>
        /// <param name="readModel">The read model.</param>
        /// <param name="event">The event.</param>
        /// <param name="context">The context of the embedding projection.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> with <see cref="EmbeddingResult{TReadModel}" />.</returns>
        Task<Try<EmbeddingResult<TReadModel>>> TryOn(TReadModel readModel, object @event, EmbeddingProjectContext context);

        /// <summary>
        /// Gets the <see cref="EventType" />.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <returns>The <see cref="EventType" />.</returns>
        EventType GetEventType(IEventTypes eventTypes);
    }
}
