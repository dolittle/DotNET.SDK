// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Dolittle.SDK.Services.given;
using Dolittle.SDK.Services.given.ReverseCall;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.SDK.Services.for_MethodCaller.given
{
    public class a_reverse_call_client : a_method_caller
    {
        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> client;

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> ReverseCallClientWith(
            ConnectArguments arguments,
            IEnumerable<ServerMessage> serverToClientMessages)
            => ReverseCallClientWith(arguments, serverToClientMessages.ToObservable());

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> ReverseCallClientWith(
            ConnectArguments arguments,
            IObservable<ServerMessage> serverToClientMessages)
            => ReverseCallClientWith(arguments, TimeSpan.FromSeconds(1), serverToClientMessages);

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> ReverseCallClientWith(
            ConnectArguments arguments,
            TimeSpan pingInterval,
            IObservable<ServerMessage> serverToClientMessages)
            => new ReverseCallClient<ClientMessage, ServerMessage, ConnectArguments, ConnectResponse, Request, Response>(
                arguments,
                Mock.Of<IReverseCallHandler<Request, Response>>(),
                Mock.Of<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>(),
                new Converter(),
                pingInterval,
                MethodCallerThatRepliesWith(serverToClientMessages),
                executionContextManager,
                Mock.Of<ILogger>());
    }
}