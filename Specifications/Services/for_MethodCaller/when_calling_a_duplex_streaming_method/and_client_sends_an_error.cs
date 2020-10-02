// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_MethodCaller.when_calling_a_duplex_streaming_method
{
    public class and_client_sends_an_error : given.a_duplex_streaming_method_and_streams
    {
        static ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method;
        static MethodCaller caller;
        static Exception thrownException;
        static IList<ServerMessage> serverToClientMessages;
        static Exception exception;

        Establish context = () =>
        {
            serverToClientMessages = new List<ServerMessage>(new[]
                {
                    new ServerMessage(),
                    new ServerMessage(),
                });

            thrownException = new Exception("Something went wrong");

            method = ADuplexStreamingMethodFrom(clientStreamWriter, AStreamReaderFrom(serverToClientMessages));

            caller = new MethodCaller("høst", 1000);
        };

        Because of = () => exception = caller.Call(method, Observable.Throw<ClientMessage>(thrownException), CancellationToken.None).SubscribeAndCatchError();

        It should_make_the_call_with_the_correct_host_and_port = () => providedChannel.ResolvedTarget.ShouldEqual("høst:1000");
        It should_return_an_error = () => exception.ShouldEqual(thrownException);
    }
}