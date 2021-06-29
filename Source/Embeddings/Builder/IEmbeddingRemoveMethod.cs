// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Defines an embedding compare-method.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public interface IEmbeddingRemoveMethod<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Invokes the compare method.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="context">The context of the embedding.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> with <see cref="UncommittedEvents" />.</returns>
        Task<Try<UncommittedEvents>> TryRemove(TReadModel currentState, EmbeddingContext context);
    }
}
