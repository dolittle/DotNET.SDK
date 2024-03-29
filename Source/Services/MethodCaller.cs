// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services;

/// <summary>
/// An implementation of <see cref="IPerformMethodCalls"/>.
/// </summary>
public class MethodCaller : IPerformMethodCalls
{
    readonly ChannelBase _channel;
    readonly string _host;
    readonly ushort _port;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodCaller"/> class.
    /// </summary>
    /// <param name="host">The host to connect to for performing method calls.</param>
    /// <param name="port">The port to connect to for performing method calls.</param>
    /// <param name="channel">The underlying channel.</param>
    public MethodCaller(ChannelBase channel, string host, ushort port)
    {
        _channel = channel;
        _host = host;
        _port = port;
    }

    /// <inheritdoc/>
    public AsyncDuplexStreamingCall<TClientMessage, TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage> method, CancellationToken token)
        where TClientMessage : IMessage
        where TServerMessage : IMessage
    {
        try
        {
            var originalStream = method.Call(_channel, CreateCallOptions(token));
            return new AsyncDuplexStreamingCall<TClientMessage, TServerMessage>(
                new CatchingClientStreamWriter<TClientMessage>(_host, _port, originalStream.RequestStream),
                new CatchingAsyncStreamReader<TServerMessage>(_host, _port, originalStream.ResponseStream),
                originalStream.ResponseHeadersAsync,
                originalStream.GetStatus,
                originalStream.GetTrailers,
                originalStream.Dispose);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            throw new CouldNotConnectToRuntime(_host, _port);
        }
    }

    /// <inheritdoc />
    public IServerStreamingEnumerable<TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallAServerStreamingMethod<TClientMessage, TServerMessage> method, TClientMessage request, CancellationToken token)
        where TClientMessage : IMessage
        where TServerMessage : IMessage
    {
        try
        {
            var originalStream = method.Call(request, _channel, CreateCallOptions(token));
            return new ServerStreamingEnumerable<TServerMessage>(
                new AsyncServerStreamingCall<TServerMessage>(
                    new CatchingAsyncStreamReader<TServerMessage>(_host, _port, originalStream.ResponseStream),
                    originalStream.ResponseHeadersAsync,
                    originalStream.GetStatus,
                    originalStream.GetTrailers,
                    originalStream.Dispose));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            throw new CouldNotConnectToRuntime(_host, _port);
        }
    }

    /// <inheritdoc/>
    public async Task<TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallAUnaryMethod<TClientMessage, TServerMessage> method, TClientMessage request, CancellationToken token)
        where TClientMessage : IMessage
        where TServerMessage : IMessage
    {
        try
        {
            return await method.Call(request, _channel, CreateCallOptions(token)).ResponseAsync.ConfigureAwait(false);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
        {
            throw new CouldNotConnectToRuntime(_host, _port);
        }
    }

    static CallOptions CreateCallOptions(CancellationToken token) => new(cancellationToken: token);
}
