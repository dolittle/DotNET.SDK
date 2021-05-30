// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;
using Microsoft.Reactive.Testing;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_subscribing
{
    [Ignore("because thread needs to sleep")]
    public class and_server_closes_connection_without_sending_any_messages : given.a_reverse_call_client
    {
        static ConnectArguments arguments;

        static ITestableObservable<ServerMessage> serverToClientMessages;
        static ITestableObserver<ConnectResponse> observer;

        Establish context = () =>
        {
            arguments = new ConnectArguments();

            serverToClientMessages = scheduler.CreateHotObservable(
                OnCompleted<ServerMessage>(200));

            client = reverse_call_client_with(arguments, serverToClientMessages);
        };

        Because of = () => observer = scheduler.Start(
            () => reverse_call_client_with(arguments, serverToClientMessages),
            created: 0,
            subscribed: 0,
            disposed: 1000);

        It should_send_the_arguments = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext<ClientMessage>(2, _ => _.Arguments == arguments),
                OnCompleted<ClientMessage>(200),
            },
            messagesSentToServer.Messages);

        It should_receive_an_error = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnError<ConnectResponse>(200, _ => _ is DidNotReceiveConnectResponse),
            },
            observer.Messages);
    }
}