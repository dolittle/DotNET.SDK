// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Represents a wrapper of a gRPC duplex streaming method call.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    public interface ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage>
        where TClientMessage : IMessage
        where TServerMessage : IMessage
    {
        /// <summary>
        /// Performs the duplex streaming method call on the server using the provided channel and call options.
        /// </summary>
        /// <param name="channel">The <see cref="Channel"/> to use to connect to the server.</param>
        /// <param name="callOptions">The <see cref="CallOptions"/> to use for the call.</param>
        /// <returns>The <see cref="AsyncDuplexStreamingCall{TRequest, TResponse}"/> representing the method call.</returns>
        AsyncDuplexStreamingCall<TClientMessage, TServerMessage> Call(Channel channel, CallOptions callOptions);
    }
}
