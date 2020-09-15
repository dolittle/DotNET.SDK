// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_connected
{
    public class and_server_does_not_send_a_ping_before_the_deadline : given.a_reverse_call_client
    {
        static ConnectArguments arguments;

        static Exception exception;

        Establish context = () =>
        {
            arguments = new ConnectArguments();
            var response = new ConnectResponse(arguments);
            var messages = new[] { new ServerMessage { Response = response } };

            var serverToClientMessages = messages.ToObservable().Merge(Observable.Empty<ServerMessage>().Delay(TimeSpan.FromMilliseconds(50)));

            client = reverse_call_client_with(arguments, TimeSpan.FromMilliseconds(10), serverToClientMessages);
        };

        Because of = () => exception = client.SubscribeAndCatchError();

        It should_send_one_message = () => messagesSentToServer.Count().ShouldEqual(1);
        It should_send_the_arguments_as_the_first_message = () => messagesSentToServer.First().ShouldMatch(_ => _.Arguments == arguments);
        It should_return_an_error = () => exception.ShouldBeOfExactType<PingTimedOut>();
    }
}