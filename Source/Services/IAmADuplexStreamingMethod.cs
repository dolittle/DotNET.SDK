﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Represents a wrapper of a gRPC duplex streaming method call.
    /// </summary>
    /// <typeparam name="TClient">The type of generated gRPC client to use.</typeparam>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    public interface IAmADuplexStreamingMethod<TClient, TClientMessage, TServerMessage>
        where TClient : ClientBase<TClient>
        where TClientMessage : IMessage
        where TServerMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <typeparamref name="TClient"/> client given a <see cref="Channel"/>.
        /// </summary>
        /// <param name="channel">The <see cref="Channel"/> to use to connect to the server.</param>
        /// <returns>A <typeparamref name="TClient"/> client that can be used to perform the call.</returns>
        TClient Client(Channel channel);

        /// <summary>
        /// Performs the duplex streaming method call on the server.
        /// </summary>
        /// <param name="client">The <typeparamref name="TClient"/> client to use to perform the call.</param>
        /// <param name="callOptions">The <see cref="CallOptions"/> to use for the call.</param>
        /// <returns>The <see cref="AsyncDuplexStreamingCall{TRequest, TResponse}"/> representing the method call.</returns>
        AsyncDuplexStreamingCall<TClientMessage, TServerMessage> Method(TClient client, CallOptions callOptions);
    }
}