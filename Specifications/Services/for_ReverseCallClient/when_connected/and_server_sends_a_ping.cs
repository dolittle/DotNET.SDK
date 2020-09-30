// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Services.given.ReverseCall;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using Microsoft.Reactive.Testing;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_connected
{
    public class and_server_sends_a_ping : given.a_reverse_call_client
    {
        static ConnectArguments arguments;
        static ConnectResponse response;

        static ITestableObservable<ServerMessage> serverToClientMessages;
        static ITestableObserver<ConnectResponse> observer;

        Establish context = () =>
        {
            arguments = new ConnectArguments();
            response = new ConnectResponse(arguments);

            serverToClientMessages = scheduler.CreateHotObservable(
                OnNext(100, new ServerMessage { Response = response }),
                OnNext(110, new ServerMessage { Ping = new Ping() }),
                OnCompleted<ServerMessage>(120));

            client = reverse_call_client_with(arguments, serverToClientMessages);
        };

        Because of = () => observer = scheduler.Start(
            () => reverse_call_client_with(arguments, TimeSpan.FromTicks(10), serverToClientMessages),
            created: 0,
            subscribed: 0,
            disposed: 1000);

        It should_send_the_arguments_and_a_pong_to_the_server = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext<ClientMessage>(2, _ => _.Arguments == arguments),
                OnNext<ClientMessage>(110, _ => _.Pong != null),
                OnCompleted<ClientMessage>(120),
            },
            messagesSentToServer.Messages);

        It should_receive_the_response = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext(100, response),
                OnCompleted<ConnectResponse>(120),
            },
            observer.Messages);
    }
}