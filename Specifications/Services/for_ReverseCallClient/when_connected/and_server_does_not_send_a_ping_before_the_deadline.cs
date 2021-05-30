// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;
using Microsoft.Reactive.Testing;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_connected
{
    [Ignore("because thread needs to sleep")]
    public class and_server_does_not_send_a_ping_before_the_deadline : given.a_reverse_call_client
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
                OnCompleted<ServerMessage>(500));
        };

        Because of = () => observer = scheduler.Start(
            () => reverse_call_client_with(arguments, TimeSpan.FromTicks(10), serverToClientMessages),
            created: 0,
            subscribed: 0,
            disposed: 1000);

        It should_send_the_arguments_and_the_error_to_the_server = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext<ClientMessage>(2, _ => _.Arguments == arguments),
                OnError<ClientMessage>(131, _ => _ is PingTimedOut),
            },
            messagesSentToServer.Messages);

        It should_receive_the_response_and_error = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext(100, response),
                OnError<ConnectResponse>(131, _ => _ is PingTimedOut),
            },
            observer.Messages);
    }
}