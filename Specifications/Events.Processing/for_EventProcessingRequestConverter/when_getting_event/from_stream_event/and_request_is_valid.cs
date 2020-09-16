// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.when_getting_event.from_stream_event
{
    public class and_request_is_valid : given.all_dependencies
    {
        static object result;

        Because of = () => result = converter.GetCLREvent(new StreamEvent { Event = committed_event });

        It should_get_the_correct_type_of_event = () => result.ShouldBeOfExactType<given.an_event>();
        It should_return_the_same_event = () => (result as given.an_event).ShouldEqual(@event);
    }
}