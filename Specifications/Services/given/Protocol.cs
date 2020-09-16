// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Services.given.ReverseCall;
using Dolittle.Services.Contracts;
using Grpc.Core;

namespace Dolittle.SDK.Services.given
{
    public class Protocol : IAmAReverseCallProtocol<ClientMessage, ServerMessage, ConnectArguments, ConnectResponse, Request, Response>
    {
        public AsyncDuplexStreamingCall<ClientMessage, ServerMessage> Call(Channel channel, CallOptions callOptions)
            => Moq.Mock.Of<AsyncDuplexStreamingCall<ClientMessage, ServerMessage>>();

        public ClientMessage CreateMessageFrom(ConnectArguments arguments) => new ClientMessage { Arguments = arguments };

        public ClientMessage CreateMessageFrom(Pong pong) => new ClientMessage { Pong = pong };

        public ClientMessage CreateMessageFrom(Response response) => new ClientMessage { Response = response };

        public ConnectResponse GetConnectResponseFrom(ServerMessage message) => message.Response;

        public Ping GetPingFrom(ServerMessage message) => message.Ping;

        public ReverseCallRequestContext GetRequestContextFrom(Request message) => message.Context;

        public Request GetRequestFrom(ServerMessage message) => message.Request;

        public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, ConnectArguments arguments) => arguments.Context = context;

        public void SetResponseContextIn(ReverseCallResponseContext context, Response response) => response.Context = context;
    }
}