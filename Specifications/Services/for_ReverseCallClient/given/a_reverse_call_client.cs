// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Dolittle.SDK.Artifacts.given;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.SDK.Artifacts.for_MethodCaller.given
{
    public class a_reverse_call_client : a_method_caller
    {
        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> client;

        protected static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> ReverseCallClientWith(
            ConnectArguments arguments,
            IEnumerable<ServerMessage> serverToClientMessages)
            => new ReverseCallClient<ClientMessage, ServerMessage, ConnectArguments, ConnectResponse, Request, Response>(
                arguments,
                Mock.Of<IReverseCallHandler<Request, Response>>(),
                Mock.Of<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>(),
                new Converter(),
                TimeSpan.FromSeconds(1),
                MethodCallerThatRepliesWith(serverToClientMessages.ToObservable()),
                executionContextManager,
                Mock.Of<ILogger>());
    }
}