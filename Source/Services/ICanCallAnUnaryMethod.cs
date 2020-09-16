// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Represents a wrapper of a gRPC unary method call.
    /// </summary>
    /// <typeparam name="TClient">The type of generated gRPC client to use.</typeparam>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    public interface ICanCallAnUnaryMethod<TClient, TClientMessage, TServerMessage>
        where TClient : ClientBase<TClient>
        where TClientMessage : IMessage
        where TServerMessage : IMessage
    {
        /// <summary>
        /// Performs an async unary call on the server using the provided message, channel and call options.
        /// </summary>
        /// <param name="message">The message to send to the server.</param>
        /// <param name="channel">The <see cref="Channel"/> to use to connect to the server.</param>
        /// <param name="callOptions">The <see cref="CallOptions"/> to use for the call.</param>
        /// <returns>A <see cref="AsyncUnaryCall{TResult}"/> representing the result of the async unary call.</returns>
        AsyncUnaryCall<TServerMessage> Call(TClientMessage message, Channel channel, CallOptions callOptions);
    }
}
