// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_MethodCaller.when_subscribing
{
    public class and_server_does_not_reply : given.a_reverse_call_client
    {
        static Exception exception;

        Establish context = () => client = ReverseCallClientWith(new ConnectArguments(), Enumerable.Empty<ServerMessage>());

        Because of = () => exception = client.CatchError();

        It should_have_sent_one_message = () => messagesSentToServer.Count().ShouldEqual(1);
        It should_return_an_error = () => exception.ShouldBeOfExactType<DidNotReceiveConnectResponse>();
    }
}