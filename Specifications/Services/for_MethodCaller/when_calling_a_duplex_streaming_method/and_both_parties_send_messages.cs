// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Artifacts.for_MethodCaller.when_calling_a_duplex_streaming_method
{
    public class and_both_parties_send_messages : given.a_duplex_streaming_method_and_streams
    {
        static ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method;
        static MethodCaller caller;
        static Exception thrownException;
        static IList<ClientMessage> clientToServerMessages;
        static Exception exception;

        Establish context = () =>
        {
            clientToServerMessages = new List<ClientMessage>(new[]
                {
                    new ClientMessage(),
                    new ClientMessage(),
                });

            thrownException = new Exception("Something went wrong");

            var serverStreamReader = new Mock<IAsyncStreamReader<ServerMessage>>();
            serverStreamReader.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Throws(thrownException);

            method = ADuplexStreamingMethodFrom(clientStreamWriter, serverStreamReader.Object);

            caller = new MethodCaller("host", 1000);
        };

        Because of = () => exception = caller.Call(method, clientToServerMessages.ToObservable()).CatchError();

        It should_make_the_call_with_the_correct_host_and_port = () => providedChannel.ResolvedTarget.ShouldEqual("host:1000");
        It should_return_an_error = () => exception.ShouldEqual(thrownException);
    }
}