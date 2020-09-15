// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Services.given.ReverseCall;
using Dolittle.Services.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_connected
{
    public class and_server_sends_a_ping : given.a_reverse_call_client
    {
        static ConnectArguments arguments;
        static ConnectResponse response;

        static IEnumerable<ConnectResponse> receivedResponses;

        Establish context = () =>
        {
            arguments = new ConnectArguments();
            response = new ConnectResponse(arguments);

            var serverToClientMessages = new[]
            {
                new ServerMessage { Response = response },
                new ServerMessage { Ping = new Ping() },
            };

            client = reverse_call_client_with(arguments, serverToClientMessages);
        };

        Because of = () => receivedResponses = client.ToArray().Wait();

        It should_return_one_response = () => receivedResponses.Count().ShouldEqual(1);
        It should_return_the_correct_response = () => receivedResponses.First().ShouldEqual(response);
        It should_send_two_messages = () => messagesSentToServer.Count().ShouldEqual(2);
        It should_send_the_arguments_as_the_first_message = () => messagesSentToServer.First().ShouldMatch(_ => _.Arguments == arguments);
        It should_send_a_pong_as_the_second_message = () => messagesSentToServer.Skip(1).First().ShouldMatch(_ => _.Pong != null);
    }
}