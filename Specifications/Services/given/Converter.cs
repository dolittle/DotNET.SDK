// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Services.given.ReverseCall;
using Dolittle.Services.Contracts;

namespace Dolittle.SDK.Services.given
{
    public class Converter : IConvertReverseCallMessages<ClientMessage, ServerMessage, ConnectArguments, ConnectResponse, Request, Response>
    {
        public ClientMessage CreateMessageFrom(ConnectArguments arguments) => new ClientMessage { Arguments = arguments };

        public ClientMessage CreateMessageFrom(Pong pong) => new ClientMessage { Pong = pong };

        public ClientMessage CreateMessageFrom(Response response) => new ClientMessage { Response = response };

        public ConnectResponse GetConnectResponseFrom(ServerMessage message) => message.Response;

        public Failure GetFailureFromConnectResponse(ConnectResponse response) => response.Failure;

        public Ping GetPingFrom(ServerMessage message) => message.Ping;

        public ReverseCallRequestContext GetRequestContextFrom(Request message) => message.Context;

        public Request GetRequestFrom(ServerMessage message) => message.Request;

        public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, ConnectArguments arguments) => arguments.Context = context;

        public void SetResponseContextIn(ReverseCallResponseContext context, Response response) => response.Context = context;
    }
}