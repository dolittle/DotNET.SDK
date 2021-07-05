// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Defines an embedding compare-method.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public interface ICompareMethod<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Invokes the compare method.
        /// </summary>
        /// <param name="receivedState">The received state.</param>
        /// <param name="currentState">The current state.</param>
        /// <param name="context">The context of the embedding.</param>
        /// <returns>A <see cref="Try{TResult}" /> with <see cref="IEnumerable{T}" /> of <see cref="object" />.</returns>
        Try<IEnumerable<object>> TryCompare(TReadModel receivedState, TReadModel currentState, EmbeddingContext context);
    }
}
