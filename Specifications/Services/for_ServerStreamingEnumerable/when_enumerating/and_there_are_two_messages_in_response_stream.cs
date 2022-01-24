// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Services.for_ServerStreamingEnumerable.given;
using Dolittle.SDK.Services.given;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ServerStreamingEnumerable.when_enumerating;

public class and_there_are_two_messages_in_response_stream : given.all_dependencies
{
    static ServerStreamingEnumerable<Message> enumerable;
    static IEnumerable<Message> messages_in_response_stream;
    static IEnumerable<Message> result;
    Establish context = () =>
    {
        messages_in_response_stream = new[]
        {
            new Message(41),
            new Message(52)
        };
        enumerable = create_enumerable(new FakeAsyncStreamReader<Message>(messages_in_response_stream));
    };

    Because of = () => result = enumerable.ToEnumerable();

    It should_get_all_messages_in_stream = () => result.ShouldContainOnly(messages_in_response_stream);
}