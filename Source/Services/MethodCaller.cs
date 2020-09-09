// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
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
        readonly int _port;
        readonly ChannelOption[] _channelOptions;
        readonly ChannelCredentials _channelCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCaller"/> class.
        /// </summary>
        /// <param name="host">The host to connect to for performing method calls.</param>
        /// <param name="port">The port to connect to for performing method calls.</param>
        public MethodCaller(string host, int port)
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
        public IObservable<TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage> method, IObservable<TClientMessage> requests)
            where TClientMessage : IMessage
            where TServerMessage : IMessage
            => Observable.Create<TServerMessage>((observer, token) =>
                {
                    var call = method.Call(CreateChannel(), CreateCallOptions(token));
                    SendMessagesToServer(requests, call.RequestStream);
                    return ReceiveAllMessagesFromServer(observer, call.ResponseStream, token);
                });

        Channel CreateChannel() => new Channel(_host, _port, _channelCredentials, _channelOptions);

        CallOptions CreateCallOptions(CancellationToken token) => new CallOptions(cancellationToken: token);

        void SendMessagesToServer<T>(IObservable<T> messages, IClientStreamWriter<T> writer)
            => messages
                .Select(message => Observable.FromAsync(() => writer.WriteAsync(message)))
                .Concat()
                .Concat(Observable.FromAsync(() => writer.CompleteAsync()))
                .Subscribe();

        async Task ReceiveAllMessagesFromServer<T>(IObserver<T> observer, IAsyncStreamReader<T> reader, CancellationToken token)
        {
            try
            {
                while (await reader.MoveNext(token).ConfigureAwait(false))
                {
                    observer.OnNext(reader.Current);
                }
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }

            observer.OnCompleted();
        }
    }
}