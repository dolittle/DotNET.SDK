// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.SDK.Services;

/// <summary>
/// Represents an implementation of <see cref="IAsyncStreamReader{T}"/> that catches RPC exceptions and throws Dolittle SDK exceptions.
/// </summary>
/// <typeparam name="T">The message type.</typeparam>
public class CatchingAsyncStreamReader<T> : IAsyncStreamReader<T>
{
    readonly string _host;
    readonly ushort _port;
    readonly IAsyncStreamReader<T> _original;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatchingAsyncStreamReader{T}"/> class.
    /// </summary>
    /// <param name="host">The host that the writer is attempting to connect to.</param>
    /// <param name="port">The port that the writer is attempting to connect to.</param>
    /// <param name="original">The original <see cref="IAsyncStreamReader{T}"/> instance to catch from.</param>
    public CatchingAsyncStreamReader(string host, ushort port, IAsyncStreamReader<T> original)
    {
        _host = host;
        _port = port;
        _original = original;
    }

    /// <inheritdoc />
    public T Current
    {
        get
        {
            try
            {
                return _original.Current;
            }
            catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
            {
                throw new CouldNotConnectToRuntime(_host, _port);
            }
        }
    }

    /// <inheritdoc />
    public async Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        try
        {
            return await _original.MoveNext(cancellationToken).ConfigureAwait(false);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
        {
            throw new CouldNotConnectToRuntime(_host, _port);
        }
    }
}