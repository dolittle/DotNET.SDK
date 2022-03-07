// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// An implementation of <see cref="IDeleteMethod{TReadModel}" />.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
public class DeleteMethod<TReadModel> : IDeleteMethod<TReadModel>
    where TReadModel : class, new()
{
    readonly DeleteEnumerableReturnSignature<TReadModel> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteMethod{TReadModel}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="DeleteEnumerableReturnSignature{TReadModel}" />.</param>
    public DeleteMethod(DeleteEnumerableReturnSignature<TReadModel> method)
    {
        _method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteMethod{TReadModel}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
    public DeleteMethod(DeleteSignature<TReadModel> method)
        : this((currentState, context) => new[] { method(currentState, context) })
    {
    }

    /// <inheritdoc/>
    public Try<IEnumerable<object>> TryDelete(TReadModel currentState, EmbeddingContext context)
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