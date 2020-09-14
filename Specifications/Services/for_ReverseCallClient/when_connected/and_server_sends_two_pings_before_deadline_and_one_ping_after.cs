// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Services.given.ReverseCall;
using Dolittle.Services.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_MethodCaller.when_subscribing
{
    public class and_server_sends_two_pings_before_deadline_and_one_ping_after : given.a_reverse_call_client
    {
        static ConnectArguments arguments;

        static Exception exception;

        Establish context = () =>
        {
            arguments = new ConnectArguments();
            var response = new ConnectResponse(arguments);
            var messages = new[] { new ServerMessage { Response = response } };

            var serverToClientMessages = messages.ToObservable()
                .Merge(new[] { new ServerMessage { Ping = new Ping() } }.ToObservable().Delay(TimeSpan.FromMilliseconds(10)))
                .Merge(new[] { new ServerMessage { Ping = new Ping() } }.ToObservable().Delay(TimeSpan.FromMilliseconds(20)))
                .Merge(new[] { new ServerMessage { Ping = new Ping() } }.ToObservable().Delay(TimeSpan.FromMilliseconds(80)));

            client = ReverseCallClientWith(arguments, TimeSpan.FromMilliseconds(10), serverToClientMessages);
        };

        Because of = () => exception = client.CatchError();

        It should_return_an_error = () => exception.ShouldBeOfExactType<PingTimedOut>();
        It should_have_sent_three_messages = () => messagesSentToServer.Count().ShouldEqual(3);
        It should_send_the_arguments_as_the_first_message = () => messagesSentToServer.First().ShouldMatch(_ => _.Arguments == arguments);
        It should_send_a_pong_as_the_second_message = () => messagesSentToServer.Skip(1).First().ShouldMatch(_ => _.Pong != null);
        It should_send_a_pong_as_the_third_message = () => messagesSentToServer.Skip(2).First().ShouldMatch(_ => _.Pong != null);
    }
}