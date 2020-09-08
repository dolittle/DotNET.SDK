// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Grpc.Core;

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
        /// <param name="method">The method that will be called on the server to initiate the reverse call.</param>
        /// <param name="converter">The converter that will be used to construct and deconstruct <typeparamref name="TClientMessage"/> and <typeparamref name="TServerMessage"/>.</param>
        /// <typeparam name="TClient">The type of generated gRPC client to use.</typeparam>
        /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
        /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
        /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
        /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
        /// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
        /// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
        /// <returns>A new reverse call client.</returns>
        IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> Create<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            TConnectArguments arguments,
            IReverseCallHandler<TRequest, TResponse> handler,
            IAmADuplexStreamingMethod<TClient, TClientMessage, TServerMessage> method,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> converter)
            where TClient : ClientBase<TClient>
            where TClientMessage : IMessage
            where TServerMessage : IMessage
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class;
    }
}