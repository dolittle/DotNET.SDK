// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// An implementation of <see cref="ICompareMethod{TEmbedding}" /> that invokes a the compare method on an embedding instance.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the projection.</typeparam>
    public class ClassCompareMethod<TEmbedding> : ICompareMethod<TEmbedding>
        where TEmbedding : class, new()
    {
        readonly CompareMethodEnumerableReturnSignature<TEmbedding> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassCompareMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="CompareMethodSignature{TEmbedding}"/> method to invoke.</param>
        public ClassCompareMethod(CompareMethodSignature<TEmbedding> method)
            : this(
                (TEmbedding instanceAndCurrentState, TEmbedding receivedState, EmbeddingContext context) => new[] { method(instanceAndCurrentState, receivedState, context) })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassCompareMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="CompareMethodEnumerableReturnSignature{TEmbedding}"/> method to invoke.</param>
        public ClassCompareMethod(CompareMethodEnumerableReturnSignature<TEmbedding> method)
        {
            _method = method;
        }

        /// <inheritdoc/>
        public Try<IEnumerable<object>> TryCompare(TEmbedding receivedState, TEmbedding currentState, EmbeddingContext context)
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
