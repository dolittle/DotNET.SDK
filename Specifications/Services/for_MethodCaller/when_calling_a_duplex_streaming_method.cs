// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Artifacts.given;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_MethodCaller
{
    public class when_calling_a_duplex_streaming_method
    {
        static Method method;
        static MethodCaller caller;
        static IEnumerable<ClientMessage> clientToServerMessages;
        static IEnumerable<ServerMessage> serverToClientMessages;

        Establish context = () =>
        {
            method = new Method(new[]
                {
                    new ServerMessage(),
                    new ServerMessage(),
                    new ServerMessage(),
                });

            clientToServerMessages = new[]
            {
                new ClientMessage(),
                new ClientMessage(),
            };

            caller = new MethodCaller("host", 1000);
        };

        Because of = () => serverToClientMessages = caller.Call(method, clientToServerMessages.ToObservable()).ToArray().Wait();

        It should_make_a_single_call = () => method.Calls.Count().ShouldEqual(1);
        It should_make_the_call_with_the_correc_host_and_port = () => method.Calls.ShouldContain(_ => _.channel.ResolvedTarget == "host:1000");
        It should_send_all_the_client_messages = () => method.ClientToServerMessages.ShouldContainOnly(clientToServerMessages);
        It should_receive_all_the_server_messages = () => serverToClientMessages.ShouldContainOnly(method.ServerToClientMessages);
    }
}