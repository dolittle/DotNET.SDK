// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a reverse call protocol.
/// </summary>
/// <typeparam name="TClientMessage">Type of the messages that is sent from the client to the server.</typeparam>
/// <typeparam name="TServerMessage">Type of the messages that is sent from the server to the client.</typeparam>
/// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
/// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
/// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
public interface IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage>, IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>, IDisconnectProtocol<TClientMessage, TServerMessage>
    where TClientMessage : IMessage
    where TServerMessage : IMessage
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
    
}