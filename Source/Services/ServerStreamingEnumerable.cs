// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services;

/// <summary>
/// Represents an implementation of <see cref="IServerStreamingEnumerable{TServerMessage}"/>.
/// </summary>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
public class ServerStreamingEnumerable<TServerMessage> : IServerStreamingEnumerable<TServerMessage>
{
    readonly AsyncServerStreamingCall<TServerMessage> _call;
    bool _disposed;
    bool _enumerated;

    /// <summary>
    /// Initializes an instance of the <see cref="ServerStreamingEnumerable{TServerMessage}"/> class.
    /// </summary>
    /// <param name="call">The <see cref="AsyncServerStreamingCall{TResponse}"/>.</param>
    public ServerStreamingEnumerable(AsyncServerStreamingCall<TServerMessage> call)
    {
        _call = call;
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        _disposed = true;
        _call?.Dispose();   
    }

    /// <inheritdoc />
    public IAsyncEnumerator<TServerMessage> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new CannotUseDisposedServerStreamingEnumerable();
        }
        if (_enumerated)
        {
            throw new CannotEnumerateServerStreamingCallMultipleTimes();
        }
        _enumerated = true;

        return new ServerStreamingEnumerator(this, _call, cancellationToken);
    }
    
    class ServerStreamingEnumerator : IAsyncEnumerator<TServerMessage>
    {
        readonly ServerStreamingEnumerable<TServerMessage> _parentEnumerable;
        readonly AsyncServerStreamingCall<TServerMessage> _call;
        readonly CancellationToken _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerStreamingEnumerable{TServerMessage}"/> class.
        /// </summary>
        /// <param name="parentEnumerable">The <see cref="ServerStreamingEnumerable{TServerMessage}"/> that owns this <see cref="ServerStreamingEnumerator"/>, which will be disposed of when this enumerator is disposed.</param>
        /// <param name="call">The <see cref="AsyncServerStreamingCall{TResponse}"/>.</param>
        /// <param name="token">The <see cref="CancellationToken"/> for cancelling the enumeration.</param>
        public ServerStreamingEnumerator(ServerStreamingEnumerable<TServerMessage> parentEnumerable, AsyncServerStreamingCall<TServerMessage> call, CancellationToken token)
        {
            _parentEnumerable = parentEnumerable;
            _call = call;
            _token = token;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            _parentEnumerable.Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        /// <inheritdoc />
        public ValueTask<bool> MoveNextAsync()
            => new(_call.ResponseStream.MoveNext(_token));

        /// <inheritdoc />
        public TServerMessage Current => _call.ResponseStream.Current;
    }
}
