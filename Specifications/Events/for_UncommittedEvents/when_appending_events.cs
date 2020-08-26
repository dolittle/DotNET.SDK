// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Events.for_UncommittedEvents
{
    public class when_appending_events : given.Events
    {
        static UncommittedEvents events;
        static EventSourceId event_source_one;
        static EventSourceId event_source_two;

        Establish context = () =>
        {
            event_source_one = Guid.Parse("9988efb7-332d-4018-8520-f767cde900d6");
            event_source_two = Guid.Parse("c9c74f08-615a-4594-9222-d694e2b9d93d");
            events = new UncommittedEvents();
        };

        Because of = () =>
        {
            events.Append(event_source_one, event_one);
            events.Append(event_source_two, event_two);
        };

        It should_have_events = () => events.HasEvents.ShouldBeTrue();
        It should_have_a_count_of_two = () => events.Count.ShouldEqual(2);
        It should_have_the_first_event_at_index_zero = () => events[0].Event.ShouldEqual(event_one);
        It should_have_the_first_event_source_at_index_zero = () => events[0].EventSource.ShouldEqual(event_source_one);
        It should_have_the_second_event_at_index_one = () => events[1].Event.ShouldEqual(event_two);
        It should_have_the_second_event_source_at_index_one = () => events[1].EventSource.ShouldEqual(event_source_two);
    }
}
