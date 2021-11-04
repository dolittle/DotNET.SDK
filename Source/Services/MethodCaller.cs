// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// An implementation of <see cref="IPerformMethodCalls"/>.
    /// </summary>
    public class MethodCaller : IPerformMethodCalls
    {
        readonly string _host;
        readonly ushort _port;
        readonly ChannelOption[] _channelOptions;
        readonly ChannelCredentials _channelCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCaller"/> class.
        /// </summary>
        /// <param name="host">The host to connect to for performing method calls.</param>
        /// <param name="port">The port to connect to for performing method calls.</param>
        public MethodCaller(string host, ushort port)
        {
            _host = host;
            _port = port;
            _channelOptions = new[]
            {
                new ChannelOption("grpc.keepalive_time", 1000),
                new ChannelOption("grpc.keepalive_timeout_ms", 500),
                new ChannelOption("grpc.keepalive_permit_without_calls", 1),
            };
            _channelCredentials = ChannelCredentials.Insecure;
        }

        /// <inheritdoc/>
        public AsyncDuplexStreamingCall<TClientMessage, TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage> method, CancellationToken token)
            where TClientMessage : IMessage
            where TServerMessage : IMessage
        {
            try
            {
                var originalStream = method.Call(CreateChannel(), CreateCallOptions(token));
                return new AsyncDuplexStreamingCall<TClientMessage, TServerMessage>(
                    new CatchingClientStreamWriter<TClientMessage>(_host, _port, originalStream.RequestStream),
                    new CatchingAsyncStreamReader<TServerMessage>(_host, _port, originalStream.ResponseStream),
                    originalStream.ResponseHeadersAsync,
                    originalStream.GetStatus,
                    originalStream.GetTrailers,
                    originalStream.Dispose);
            }
            catch (Exception)
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
                return await method.Call(request, CreateChannel(), CreateCallOptions(token)).ResponseAsync.ConfigureAwait(false);
            }
            catch (RpcException exception) when (exception.StatusCode == StatusCode.Unavailable)
            {
                throw new CouldNotConnectToRuntime(_host, _port);
            }
        }

        Channel CreateChannel() => new Channel(_host, _port, _channelCredentials, _channelOptions);

        CallOptions CreateCallOptions(CancellationToken token) => new CallOptions(cancellationToken: token);
    }
}
