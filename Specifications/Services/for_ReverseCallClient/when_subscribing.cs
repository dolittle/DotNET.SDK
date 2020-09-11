// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_MethodCaller
{
    public class when_subscribing : given.a_reverse_call_client
    {
        static ConnectArguments arguments;

        Establish context = () =>
        {
            arguments = new ConnectArguments();
            client = ReverseCallClientWith(arguments);
        };

        Because of = () => client.Subscribe();

        It should_have_sent_one_message = () => messagesSentToServer.Count().Wait().ShouldEqual(1);
        It should_have_sent_the_correct_arguments = () => messagesSentToServer.Take(1).Wait().Arguments.ShouldEqual(arguments);
    }
}