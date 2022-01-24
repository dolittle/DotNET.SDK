// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.SDK.Services.for_ServerStreamingEnumerable.given;
using Dolittle.SDK.Services.given;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ServerStreamingEnumerable.when_enumerating;

public class and_cancellation_is_requested : given.all_dependencies
{
    static ServerStreamingEnumerable<Message> enumerable;
    static IList<Message> result;
    Establish context = () =>
    {
        enumerable = create_enumerable(new FakeAsyncStreamReader<Message>(new Message(41), new Message(52)));
    };

    Because of = () => result = enumerable.ToListAsync(new CancellationToken(true)).GetAwaiter().GetResult();

    It should_not_get_any_messages = () => result.ShouldBeEmpty();
}