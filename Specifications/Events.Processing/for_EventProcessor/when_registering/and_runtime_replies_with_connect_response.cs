// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using Dolittle.Events.Processing.for_EventProcessor.given;
using Machine.Specifications;

namespace Dolittle.Events.Processing.for_EventProcessor.when_registering
{
    public class and_runtime_replies_with_connect_response
    {
        static ConnectArguments arguments;
        static ConnectResponse response;
        static IList<Request> requests;
        static EventProcessor processor;

        Establish context = () =>
        {
            var id = (EventProcessorId)"9bc05ac9-c4ee-4a15-bfe4-a13fd97b7341";
            arguments = new ConnectArguments();
            response = new ConnectResponse();
            requests = new List<Request>()
            {
                new Request(),
                new Request(),
            };
            processor = new EventProcessor(id, true, arguments, response, requests);
        };

        Because of = () => processor.RegisterAndHandle(CancellationToken.None).GetAwaiter().GetResult();

        It should_have_sent_the_correct_arguments = () => processor.ConnectArgumentsSent.ShouldContainOnly(arguments);
        It should_have_processed_two_requests = () => processor.ResponsesSent.Count.ShouldEqual(2);
        It should_have_processed_the_first_request_first = () => processor.ResponsesSent[0].ShouldMatch(_ => _.Request == requests[0]);
        It should_have_processed_the_second_request_second = () => processor.ResponsesSent[1].ShouldMatch(_ => _.Request == requests[1]);
    }
}