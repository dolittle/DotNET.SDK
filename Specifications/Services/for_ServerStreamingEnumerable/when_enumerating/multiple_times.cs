// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.SDK.Services.for_ServerStreamingEnumerable.given;
using Dolittle.SDK.Services.given;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ServerStreamingEnumerable.when_enumerating;

public class multiple_times : given.all_dependencies
{
    static ServerStreamingEnumerable<Message> enumerable;
    static Exception result;
    Establish context = () =>
    {
        enumerable = create_enumerable(new FakeAsyncStreamReader<Message>());
        enumerable.GetAsyncEnumerator();
    };

    Because of = () => result = Catch.Exception(() => enumerable.ForEachAsync(_ => {}).GetAwaiter().GetResult());

    It should_fail_because_it_is_already_enumerated = () => result.ShouldBeOfExactType<CannotEnumerateServerStreamingCallMultipleTimes>();
}