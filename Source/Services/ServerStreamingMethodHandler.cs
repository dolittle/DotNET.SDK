// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services;

/// <summary>
/// Represents an implementation of <see cref="IServerStreamingMethodHandler{TServerMessage}"/>.
/// </summary>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
public class ServerStreamingMethodHandler<TServerMessage> : IServerStreamingMethodHandler<TServerMessage>, IDisposable
{
    readonly AsyncServerStreamingCall<TServerMessage> _call;
    bool _disposed;

    /// <summary>
    /// Initializes an instance of the <see cref="ServerStreamingMethodHandler{TServerMessage}"/> class.
    /// </summary>
    /// <param name="call"></param>
    public ServerStreamingMethodHandler(AsyncServerStreamingCall<TServerMessage> call)
    {
        _call = call;
    }

    /// <inheritdoc />
    public Task<IEnumerable<TServerMessage>> AggregateResponses(CancellationToken token = default)
        => throw new System.NotImplementedException();

    /// <inheritdoc />
    public void Dispose()
    {
        _disposed = true;
        _call?.Dispose();   
    }
}
