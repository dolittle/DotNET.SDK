// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// An implementation of <see cref="IUpdateMethod{TReadModel}" />.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
public class UpdateMethod<TReadModel> : IUpdateMethod<TReadModel>
    where TReadModel : class, new()
{
    readonly UpdateEnumerableReturnSignature<TReadModel> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMethod{TReadModel}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="UpdateEnumerableReturnSignature{TReadModel}" />.</param>
    public UpdateMethod(UpdateEnumerableReturnSignature<TReadModel> method)
    {
        _method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMethod{TReadModel}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
    public UpdateMethod(UpdateSignature<TReadModel> method)
        : this((receivedState, currentState, context) => new[] { method(receivedState, currentState, context) })
    {
    }

    /// <inheritdoc/>
    public Try<IEnumerable<object>> TryUpdate(TReadModel receivedState, TReadModel currentState, EmbeddingContext context)
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