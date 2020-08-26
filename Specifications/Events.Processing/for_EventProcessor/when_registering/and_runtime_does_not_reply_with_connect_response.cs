// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.Events.Processing.for_EventProcessor.given;
using Machine.Specifications;

namespace Dolittle.Events.Processing.for_EventProcessor.when_registering
{
    public class and_runtime_does_not_reply_with_connect_response
    {
        static ConnectArguments arguments;
        static EventProcessor processor;
        static Exception exception;

        Establish context = () =>
        {
            arguments = new ConnectArguments();
            var requests = new List<Request>()
            {
                new Request(),
                new Request(),
            };
            processor = new EventProcessor("9bc05ac9-c4ee-4a15-bfe4-a13fd97b7341", false, arguments, new ConnectResponse(), requests);
        };

        Because of = () => exception = Catch.Exception(() => processor.RegisterAndHandle(CancellationToken.None).GetAwaiter().GetResult());

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<DidNotReceiveRegistrationResponse>();
        It should_not_process_any_requests = () => processor.ResponsesSent.ShouldBeEmpty();
    }
}