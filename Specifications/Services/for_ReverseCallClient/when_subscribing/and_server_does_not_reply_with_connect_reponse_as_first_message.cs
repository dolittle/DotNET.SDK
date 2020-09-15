// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_subscribing
{
    public class and_server_does_not_reply_with_connect_reponse_as_first_message : given.a_reverse_call_client
    {
        static Exception exception;

        Establish context = () => client = reverse_call_client_with(new ConnectArguments(), new[] { new ServerMessage { Request = new Request() } });

        Because of = () => exception = client.SubscribeAndCatchError();

        It should_have_sent_one_message = () => messagesSentToServer.Count().ShouldEqual(1);
        It should_return_an_error = () => exception.ShouldBeOfExactType<DidNotReceiveConnectResponse>();
    }
}