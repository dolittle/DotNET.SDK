// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reactive.Linq;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_MethodCaller.when_calling_a_duplex_streaming_method
{
    public class and_only_client_sends_messages : given.a_duplex_streaming_method_and_streams
    {
        static ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method;
        static MethodCaller caller;
        static IList<ClientMessage> clientToServerMessages;
        static IEnumerable<ServerMessage> receivedServerMessages;

        Establish context = () =>
        {
            clientToServerMessages = new List<ClientMessage>(new[]
                {
                    new ClientMessage(),
                    new ClientMessage(),
                    new ClientMessage(),
                    new ClientMessage(),
                    new ClientMessage(),
                });

            method = ADuplexStreamingMethodFrom(clientStreamWriter, AStreamReaderFrom(new List<ServerMessage>()));

            caller = new MethodCaller("host", 1337);
        };

        Because of = () => receivedServerMessages = caller.Call(method, clientToServerMessages.ToObservable()).ToArray().Wait();

        It should_make_the_call_with_the_correct_host_and_port = () => providedChannel.ResolvedTarget.ShouldEqual("host:1337");
        It should_send_all_the_client_messages = () => writtenClientMessages.ShouldContainOnly(clientToServerMessages);
        It should_not_receive_any_messages_from_server = () => receivedServerMessages.ShouldBeEmpty();
    }
}