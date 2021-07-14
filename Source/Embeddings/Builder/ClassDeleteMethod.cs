// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// An implementation of <see cref="IDeleteMethod{TEmbedding}" /> that invokes the delete method on an embedding instance.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the projection.</typeparam>
    public class ClassDeleteMethod<TEmbedding> : IDeleteMethod<TEmbedding>
        where TEmbedding : class, new()
    {
        readonly DeleteMethodEnumerableReturnSignature<TEmbedding> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDeleteMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="DeleteMethodSignature{TEmbedding}"/> method to invoke.</param>
        public ClassDeleteMethod(DeleteMethodSignature<TEmbedding> method)
            : this(
                (TEmbedding instanceAndCurrentState, EmbeddingContext context) => new[] { method(instanceAndCurrentState, context) })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDeleteMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="DeleteMethodEnumerableReturnSignature{TEmbedding}"/> method to invoke.</param>
        public ClassDeleteMethod(DeleteMethodEnumerableReturnSignature<TEmbedding> method)
        {
            _method = method;
        }

        /// <inheritdoc/>
        public Try<IEnumerable<object>> TryDelete(TEmbedding currentState, EmbeddingContext context)
        {
            try
            {
                return new Try<IEnumerable<object>>(_method(currentState, context));
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
