// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a server streaming method call.
/// </summary>
/// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
public interface ICanCallAServerStreamingMethod<in TClientMessage, TServerMessage>
    where TClientMessage : IMessage
    where TServerMessage : IMessage
{
    /// <summary>
    /// Performs the server streaming method call on the server using the provided channel and call options.
    /// </summary>
    /// <param name="message">The message to send to the server.</param>
    /// <param name="channel">The <see cref="Channel"/> to use to connect to the server.</param>
    /// <param name="callOptions">The <see cref="CallOptions"/> to use for the call.</param>
    /// <returns>The <see cref="AsyncDuplexStreamingCall{TRequest, TResponse}"/> representing the method call.</returns>
    AsyncServerStreamingCall<TServerMessage> Call(TClientMessage message, Channel channel, CallOptions callOptions);
}
