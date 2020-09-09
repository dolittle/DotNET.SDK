// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Google.Protobuf;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Defines a system that can perform gRPC method calls.
    /// </summary>
    public interface IPerformMethodCalls
    {
        /// <summary>
        /// Performs the provided duplex streaming method call and sends the provided requests to the server.
        /// </summary>
        /// <param name="method">The <see cref="ICanCallADuplexStreamingMethod{TClientMessage, TServerMessage}">method</see> to call.</param>
        /// <param name="requests">An <see cref="IObservable{TClientMessage}"/> of requests to send.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
        /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
        /// <returns>An <see cref="IObservable{TServerMessage}"/> of response from the server.</returns>
        IObservable<TServerMessage> Call<TClientMessage, TServerMessage>(ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage> method, IObservable<TClientMessage> requests, CancellationToken token)
            where TClientMessage : IMessage
            where TServerMessage : IMessage;
    }
}