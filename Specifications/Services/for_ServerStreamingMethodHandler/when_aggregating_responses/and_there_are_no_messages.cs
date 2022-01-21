// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Services.for_ServerStreamingMethodHandler.given;
using Dolittle.SDK.Services.given;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ServerStreamingMethodHandler.when_aggregating_responses;

public class and_there_are_no_messages : given.all_dependencies
{
    static ServerStreamingMethodHandler<Message> handler;
    static IEnumerable<Message> result;
    Establish context = () =>
    {
        handler = create_handler(new FakeAsyncStreamReader<Message>());
    };

    Because of = () => result = handler.AggregateResponses().GetAwaiter().GetResult();

    It should_not_get_any_messages = () => result.ShouldBeEmpty();
}