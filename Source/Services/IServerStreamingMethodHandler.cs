// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a system that can handle a gRPC server streaming method.
/// </summary>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
public interface IServerStreamingMethodHandler<TServerMessage> : IDisposable
{
    /// <summary>
    /// Aggregates all responses from the server streaming call.
    /// </summary>
    /// <param name="token">The optional <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of <typeparamref name="TServerMessage"/> responses from the server.</returns>
    public Task<IEnumerable<TServerMessage>> AggregateResponses(CancellationToken token = default);
}
