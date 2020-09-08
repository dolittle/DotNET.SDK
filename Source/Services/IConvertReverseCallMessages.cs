// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Services.Contracts;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Defines a converter that reads and writes parts of duplex streaming messages using the reverse call protocol.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the messages that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the messages that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
    public interface IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
    {
        /// <summary>
        /// Sets the <see cref="ReverseCallArgumentsContext"/> in a .
        /// </summary>
        /// <param name="context">The <see cref="ReverseCallArgumentsContext"/> to set.</param>
        /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to set the context in.</param>
        void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, TConnectArguments arguments);

        /// <summary>
        /// Creates <typeparamref name="TClientMessage"/> from a <typeparamref name="TConnectArguments"/>.
        /// </summary>
        /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to use.</param>
        /// <returns>A new <typeparamref name="TClientMessage"/>.</returns>
        TClientMessage CreateMessageFrom(TConnectArguments arguments);

        /// <summary>
        /// Gets the <typeparamref name="TConnectResponse"/> from a <typeparamref name="TServerMessage"/>.
        /// </summary>
        /// <param name="message">The <typeparamref name="TServerMessage"/> to get the connect response from.</param>
        /// <returns>The <typeparamref name="TConnectResponse"/> in the message.</returns>
        TConnectResponse GetConnectResponseFrom(TServerMessage message);

        /// <summary>
        /// Gets the <see cref="Ping"/> from a <typeparamref name="TServerMessage"/>.
        /// </summary>
        /// <param name="message">The <typeparamref name="TServerMessage"/> to get the ping from.</param>
        /// <returns>The <see cref="Ping"/> in the message.</returns>
        Ping GetPingFrom(TServerMessage message);

        /// <summary>
        /// Creates <typeparamref name="TClientMessage"/> from a <see cref="Pong"/>.
        /// </summary>
        /// <param name="pong">The <see cref="Pong"/> to use.</param>
        /// <returns>A new <typeparamref name="TClientMessage"/>.</returns>
        TClientMessage CreateMessageFrom(Pong pong);

        /// <summary>
        /// Gets the <typeparamref name="TRequest"/> from a <typeparamref name="TServerMessage"/>.
        /// </summary>
        /// <param name="message">The <typeparamref name="TServerMessage"/> to get the request from.</param>
        /// <returns>The <typeparamref name="TRequest"/> in the message.</returns>
        TRequest GetRequestFrom(TServerMessage message);

        /// <summary>
        /// Gets the <see cref="ReverseCallRequestContext"/> from a <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="message">The <typeparamref name="TRequest"/> to get the context from.</param>
        /// <returns>The <see cref="ReverseCallRequestContext"/> in the request.</returns>
        ReverseCallRequestContext GetRequestContextFrom(TRequest message);

        /// <summary>
        /// Sets the <see cref="ReverseCallResponseContext"/> in a <typeparamref name="TResponse"/>.
        /// </summary>
        /// <param name="context">The <see cref="ReverseCallResponseContext"/> to set.</param>
        /// <param name="response">Tge <typeparamref name="TResponse"/> to set the context in.</param>
        void SetResponseContextIn(ReverseCallResponseContext context, TResponse response);

        /// <summary>
        /// Creates <typeparamref name="TClientMessage"/> from a <typeparamref name="TResponse"/>.
        /// </summary>
        /// <param name="response">The <typeparamref name="TResponse"/> to use.</param>
        /// <returns>A new <typeparamref name="TClientMessage"/>.</returns>
        TClientMessage CreateMessageFrom(TResponse response);
    }
}