// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.SDK.Services;

/// <summary>
/// Represents an implementation of <see cref="IClientStreamWriter{T}"/> that catches RPC exceptions and throws Dolittle SDK exceptions.
/// </summary>
/// <typeparam name="T">The message type.</typeparam>
public class CatchingClientStreamWriter<T> : IClientStreamWriter<T>
{
    readonly string _host;
    readonly ushort _port;
    readonly IClientStreamWriter<T> _original;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatchingClientStreamWriter{T}"/> class.
    /// </summary>
    /// <param name="host">The host that the writer is attempting to connect to.</param>
    /// <param name="port">The port that the writer is attempting to connect to.</param>
    /// <param name="original">The original <see cref="IClientStreamWriter{T}"/> instance to catch from.</param>
    public CatchingClientStreamWriter(string host, ushort port, IClientStreamWriter<T> original)
    {
        _host = host;
        _port = port;
        _original = original;
    }

    /// <inheritdoc />
    public WriteOptions WriteOptions
    {
        get
        {
            try
            {
                return _original.WriteOptions;
            }
            catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
            {
                throw new CouldNotConnectToRuntime(_host, _port);
            }
        }

        set
        {
            try
            {
                _original.WriteOptions = value;
            }
            catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
            {
                throw new CouldNotConnectToRuntime(_host, _port);
            }
        }
    }

    /// <inheritdoc />
    public async Task WriteAsync(T message)
    {
        try
        {
            await _original.WriteAsync(message).ConfigureAwait(false);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
        {
            throw new CouldNotConnectToRuntime(_host, _port);
        }
    }

    /// <inheritdoc />
    public async Task CompleteAsync()
    {
        try
        {
            await _original.CompleteAsync().ConfigureAwait(false);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
        {
            throw new CouldNotConnectToRuntime(_host, _port);
        }
    }
}