// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// An implementation of <see cref="IUpdateMethod{TEmbedding}" /> that invokes a the update method on an embedding instance.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the projection.</typeparam>
    public class ClassUpdateMethod<TEmbedding> : IUpdateMethod<TEmbedding>
        where TEmbedding : class, new()
    {
        readonly UpdateMethodEnumerableReturnSignature<TEmbedding> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassUpdateMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="UpdateMethodSignature{TEmbedding}"/> method to invoke.</param>
        public ClassUpdateMethod(UpdateMethodSignature<TEmbedding> method)
            : this(
                (TEmbedding instanceAndCurrentState, TEmbedding receivedState, EmbeddingContext context) => new[] { method(instanceAndCurrentState, receivedState, context) })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassUpdateMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="UpdateMethodEnumerableReturnSignature{TEmbedding}"/> method to invoke.</param>
        public ClassUpdateMethod(UpdateMethodEnumerableReturnSignature<TEmbedding> method)
        {
            _method = method;
        }

        /// <inheritdoc/>
        public Try<IEnumerable<object>> TryUpdate(TEmbedding receivedState, TEmbedding currentState, EmbeddingContext context)
        {
            try
            {
                return new Try<IEnumerable<object>>(_method(currentState, receivedState, context));
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
