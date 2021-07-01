// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// An implementation of <see cref="ICompareMethod{TReadModel}" />.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
    public class CompareMethod<TReadModel> : ICompareMethod<TReadModel>
        where TReadModel : class, new()
    {
        readonly CompareEnumerableReturnSignature<TReadModel> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareMethod{TReadModel}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="CompareEnumerableReturnSignature{TReadModel}" />.</param>
        public CompareMethod(CompareEnumerableReturnSignature<TReadModel> method)
        {
            _method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareMethod{TReadModel}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
        public CompareMethod(CompareSignature<TReadModel> method)
            : this((receivedState, currentState, context) => new[] { method(receivedState, currentState, context) })
        {
        }

        /// <inheritdoc/>
        public Try<IEnumerable<object>> TryCompare(TReadModel receivedState, TReadModel currentState, EmbeddingContext context)
        {
            try
            {
                return new Try<IEnumerable<object>>(_method(receivedState, currentState, context));
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
