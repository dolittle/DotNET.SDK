// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Defines a client for reverse calls coming from the server to the client.
    /// </summary>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
    public interface IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> : IObservable<TConnectResponse>
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Gets the connect arguments that will be sent to the server when subscribed.
        /// </summary>
        TConnectArguments Arguments { get; }

        /// <summary>
        /// Gets the handler that handles requests from the server.
        /// </summary>
        IReverseCallHandler<TRequest, TResponse> Handler { get; }
    }
}