// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a client for reverse calls coming from server to client.
/// </summary>
/// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
/// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
/// <typeparam name="TResponse">Type of the responses received from the client using.</typeparam>
public interface IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse, TClientMessage>
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
    where TClientMessage: IMessage
{
    /// <summary>
    /// Gets the connect response.
    /// </summary>
    TConnectResponse ConnectResponse { get; }

    /// <summary>
    /// Connect to server.
    /// </summary>
    /// <param name="connectArguments">The connection arguments.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns whether a connection response was received.</returns>
    Task<bool> Connect(TConnectArguments connectArguments, CancellationToken token);

    /// <summary>
    /// Write a message to the server.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task WriteMessage(TClientMessage message, CancellationToken token);

    /// <summary>
    /// Handle a call.
    /// </summary>
    /// <param name="handler">The handler that will handle requests from the server.</param>
    /// <param name="token">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Handle(IReverseCallHandler<TRequest, TResponse> handler, CancellationToken token);
}