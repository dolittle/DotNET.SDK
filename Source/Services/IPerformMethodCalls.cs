// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a system that can perform gRPC method calls.
/// </summary>
public interface IPerformMethodCalls
{
    /// <summary>
    /// Performs the provided duplex streaming method call and sends the provided requests to the server.
    /// </summary>
    /// <param name="method">The <see cref="ICanCallADuplexStreamingMethod{TClientMessage, TServerMessage}">method</see> to call.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the call.</param>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <returns>An <see cref="AsyncDuplexStreamingCall{TClientMessage, TServerMessage}"/> of response from the server, that when subscribed to initiates the call.</returns>
    AsyncDuplexStreamingCall<TClientMessage, TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage> method, CancellationToken token)
        where TClientMessage : IMessage
        where TServerMessage : IMessage;

    /// <summary>
    /// Performs the provided server streaming method call.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <param name="method">The <see cref="ICanCallAServerStreamingMethod{TClientMessage, TServerMessage}"/> method to call.</param>
    /// <param name="request">The <see cref="IMessage"/> to send to the server.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the call.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    IServerStreamingMethodHandler<TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallAServerStreamingMethod<TClientMessage, TServerMessage> method, TClientMessage request, CancellationToken token)
        where TClientMessage : IMessage
        where TServerMessage : IMessage;
    
    /// <summary>
    /// Performs the provided unary method call.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <param name="method">The <see cref="ICanCallAUnaryMethod{TClientMessage, TServerMessage}"/> method to call.</param>
    /// <param name="request">The <see cref="IMessage"/> to send to the server.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the call.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallAUnaryMethod<TClientMessage, TServerMessage> method, TClientMessage request, CancellationToken token)
        where TClientMessage : IMessage
        where TServerMessage : IMessage;
}
