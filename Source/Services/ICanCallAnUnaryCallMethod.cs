// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services
{

    /// <summary>
    /// Represents a wrapper of a gRPC unary method call.
    /// </summary>
    public interface ICanCallAnUnaryCallMethod<TClient, TClientMessage, TServerMessage>
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
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the async unary call.</returns>
        Task<TServerMessage> Call(TClientMessage message, Channel channel, CallOptions callOptions);
    }
}
