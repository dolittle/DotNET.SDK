// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_MethodCaller.when_subscribing
{
    public class and_server_sends_connect_response : given.a_reverse_call_client
    {
        static ConnectArguments arguments;
        static ConnectResponse response;

        static IEnumerable<ConnectResponse> receivedResponses;

        Establish context = () =>
        {
            arguments = new ConnectArguments();
            response = new ConnectResponse(arguments);

            client = ReverseCallClientWith(arguments, new[] { new ServerMessage { Response = response } });
        };

        Because of = () => receivedResponses = client.ToArray().Wait();

        It should_have_sent_one_message = () => messagesSentToServer.Count().ShouldEqual(1);
        It should_have_sent_the_correct_arguments = () => messagesSentToServer.First().Arguments.ShouldEqual(arguments);
        It should_return_one_response = () => receivedResponses.Count().ShouldEqual(1);
        It should_return_the_correct_response = () => receivedResponses.First().ShouldEqual(response);
    }
}