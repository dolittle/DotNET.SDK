// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Defines a system that can create instances of <see cref="IReverseCallClient{TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
    /// </summary>
    public interface ICreateReverseCallClients
    {
        /// <summary>
        /// Create a reverse call client given the provided arguments, handler, method and converter.
        /// </summary>
        /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to send to the server to start the reverse call protocol.</param>
        /// <param name="handler">The handler that will handle requests from the server.</param>
        /// <param name="protocol">The protocol for this reverse call.</param>
        /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
        /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
        /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
        /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
        /// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
        /// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
        /// <returns>A new reverse call client.</returns>
        IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> Create<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            TConnectArguments arguments,
            IReverseCallHandler<TRequest, TResponse> handler,
            IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol)
            where TClientMessage : IMessage
            where TServerMessage : IMessage
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class;
    }
}