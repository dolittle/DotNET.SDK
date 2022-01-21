// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Services.for_ServerStreamingMethodHandler.given;
using Dolittle.SDK.Services.given;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ServerStreamingMethodHandler.when_aggregating_responses;

public class and_handler_is_disposed : given.all_dependencies
{
    static ServerStreamingMethodHandler<Message> handler;
    static Exception result;
    Establish context = () =>
    {
        handler = create_handler(new FakeAsyncStreamReader<Message>());
        handler.Dispose();
    };

    Because of = () => result = Catch.Exception(() => handler.AggregateResponses().GetAwaiter().GetResult());

    It should_fail_because_it_is_already_disposed = () => result.ShouldBeOfExactType<CannotUseDisposedServerStreamingMethodHandler>();
}