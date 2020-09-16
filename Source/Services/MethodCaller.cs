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
        public IObservable<TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage> method, IObservable<TClientMessage> requests)
            where TClientMessage : IMessage
            where TServerMessage : IMessage
            => Observable.Create<TServerMessage>((observer, token) =>
                {
                    var tcs = CancellationTokenSource.CreateLinkedTokenSource(token);
                    var call = method.Call(CreateChannel(), CreateCallOptions(tcs.Token));
                    SendMessagesToServer(observer, requests, call.RequestStream, tcs);
                    return ReceiveAllMessagesFromServer(observer, call.ResponseStream, tcs.Token);
                });

        /// <inheritdoc/>
        public Task<TServerMessage> Call<TClient, TClientMessage, TServerMessage>(ICanCallAnUnaryMethod<TClient, TClientMessage, TServerMessage> method, TClientMessage request, CancellationToken token)
            where TClient : ClientBase<TClient>
            where TClientMessage : IMessage
            where TServerMessage : IMessage
        {
            return method.Call(request, CreateChannel(), CreateCallOptions(token)).ResponseAsync;
        }

        Channel CreateChannel() => new Channel(_host, _port, _channelCredentials, _channelOptions);

        CallOptions CreateCallOptions(CancellationToken token) => new CallOptions(cancellationToken: token);

        void SendMessagesToServer<TClientMessage, TServerMessage>(IObserver<TClientMessage> observer, IObservable<TServerMessage> messages, IClientStreamWriter<TServerMessage> writer, CancellationTokenSource tcs)
            => messages
                .Select(message => Observable.FromAsync(() => writer.WriteAsync(message)))
                .Concat()
                .Concat(Observable.FromAsync(() => writer.CompleteAsync()))
                .Subscribe(
                    _ => { },
                    error =>
                    {
                        observer.OnError(error);
                        tcs.Cancel();
                    });

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
                return;
            }

            observer.OnCompleted();
        }
    }
}
