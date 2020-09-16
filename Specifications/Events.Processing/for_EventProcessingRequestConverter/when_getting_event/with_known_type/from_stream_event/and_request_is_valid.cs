// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.when_getting_event.with_known_type.from_stream_event
{
    public class and_request_is_valid : given.all_dependencies
    {
        static given.an_event result;

        Because of = () => result = converter.GetCLREvent<given.an_event>(new StreamEvent { Event = committed_event });

        It should_get_the_correct_type_of_event = () => result.ShouldBeOfExactType<given.an_event>();
        It should_return_the_same_event = () => result.ShouldEqual(@event);
    }
}