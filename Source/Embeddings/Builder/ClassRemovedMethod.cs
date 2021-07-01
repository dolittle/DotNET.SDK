// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// An implementation of <see cref="IRemoveMethod{TEmbedding}" /> that invokes the remove method on an embedding instance.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the projection.</typeparam>
    public class ClassRemovedMethod<TEmbedding> : IRemoveMethod<TEmbedding>
        where TEmbedding : class, new()
    {
        readonly RemoveMethodEnumerableReturnSignature<TEmbedding> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassRemovedMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="RemoveMethodSignature{TEmbedding}"/> method to invoke.</param>
        public ClassRemovedMethod(RemoveMethodSignature<TEmbedding> method)
            : this(
                (TEmbedding instanceAndCurrentState, EmbeddingContext context) => new[] { method(instanceAndCurrentState, context) })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassRemovedMethod{TEmbedding}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="RemoveMethodEnumerableReturnSignature{TEmbedding}"/> method to invoke.</param>
        public ClassRemovedMethod(RemoveMethodEnumerableReturnSignature<TEmbedding> method)
        {
            _method = method;
        }

        /// <inheritdoc/>
        public Try<IEnumerable<object>> TryRemove(TEmbedding currentState, EmbeddingContext context)
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
