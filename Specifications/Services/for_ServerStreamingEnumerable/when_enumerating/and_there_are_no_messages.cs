// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Services.for_ServerStreamingEnumerable.given;
using Dolittle.SDK.Services.given;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ServerStreamingEnumerable.when_enumerating;

public class and_there_are_no_messages : given.all_dependencies
{
    static ServerStreamingEnumerable<Message> enumerable;
    static IEnumerable<Message> result;
    Establish context = () =>
    {
        enumerable = create_enumerable(new FakeAsyncStreamReader<Message>());
    };

    Because of = () => result = enumerable.ToEnumerable();

    It should_not_get_any_messages = () => result.ShouldBeEmpty();
}