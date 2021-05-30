// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;
using Microsoft.Reactive.Testing;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_subscribing
{
    [Ignore("because thread needs to sleep")]
    public class and_server_does_not_reply_with_connect_reponse_as_first_message : given.a_reverse_call_client
    {
        static ConnectArguments arguments;

        static ITestableObservable<ServerMessage> serverToClientMessages;
        static ITestableObserver<ConnectResponse> observer;

        Establish context = () =>
        {
            arguments = new ConnectArguments();

            serverToClientMessages = scheduler.CreateHotObservable(
                OnNext(200, new ServerMessage { Request = new Request() }));

            client = reverse_call_client_with(arguments, serverToClientMessages);
        };

        Because of = () => observer = scheduler.Start(
            () => reverse_call_client_with(arguments, serverToClientMessages),
            created: 0,
            subscribed: 0,
            disposed: 1000);

        It should_send_the_arguments_and_error = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext<ClientMessage>(2, _ => _.Arguments == arguments),
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
