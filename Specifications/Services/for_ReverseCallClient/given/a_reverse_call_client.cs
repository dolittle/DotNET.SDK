// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Services.given;
using Dolittle.SDK.Services.given.ReverseCall;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.SDK.Services.for_ReverseCallClient.given
{
    public class a_reverse_call_client : a_method_caller
    {
        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> client;

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> reverse_call_client_with(
            ConnectArguments arguments,
            IObservable<ServerMessage> serverToClientMessages)
            => reverse_call_client_with(arguments, TimeSpan.FromTicks(1000), serverToClientMessages);

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> reverse_call_client_with(
            ConnectArguments arguments,
            IReverseCallHandler<Request, Response> handler,
            IObservable<ServerMessage> serverToClientMessages)
            => reverse_call_client_with(arguments, TimeSpan.FromTicks(1000), handler, serverToClientMessages);

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> reverse_call_client_with(
            ConnectArguments arguments,
            TimeSpan pingInterval,
            IObservable<ServerMessage> serverToClientMessages)
            => reverse_call_client_with(arguments, pingInterval, Mock.Of<IReverseCallHandler<Request, Response>>(), serverToClientMessages);

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> reverse_call_client_with(
            ConnectArguments arguments,
            TimeSpan pingInterval,
            IReverseCallHandler<Request, Response> handler,
            IObservable<ServerMessage> serverToClientMessages)
            => new ReverseCallClient<ClientMessage, ServerMessage, ConnectArguments, ConnectResponse, Request, Response>(
                arguments,
                handler,
                new Protocol(),
                pingInterval,
                method_caller_that_replies_with(serverToClientMessages),
                executionContext,
                scheduler,
                Mock.Of<ILogger>());
    }
}