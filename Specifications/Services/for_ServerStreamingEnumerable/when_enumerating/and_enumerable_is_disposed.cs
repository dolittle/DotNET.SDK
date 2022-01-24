// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.SDK.Services.for_ServerStreamingEnumerable.given;
using Dolittle.SDK.Services.given;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ServerStreamingEnumerable.when_enumerating;

public class and_enumerable_is_disposed : given.all_dependencies
{
    static ServerStreamingEnumerable<Message> enumerable;
    static Exception result;
    Establish context = () =>
    {
        enumerable = create_enumerable(new FakeAsyncStreamReader<Message>());
        enumerable.Dispose();
    };

    Because of = () => result = Catch.Exception(() => enumerable.ToListAsync().GetAwaiter().GetResult());

    It should_fail_because_it_is_already_disposed = () => result.ShouldBeOfExactType<CannotUseDisposedServerStreamingEnumerable>();
}