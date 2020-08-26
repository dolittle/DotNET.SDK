﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Events.for_UncommittedAggregateEvents
{
    public class when_appending_events : given.an_aggregate_and_two_events
    {
        static UncommittedAggregateEvents events;

        Establish context = () => events = new UncommittedAggregateEvents(event_source_id, aggregate_root_type, aggregate_root_version);

        Because of = () =>
        {
            events.Append(event_one);
            events.Append(event_two);
        };

        It should_have_events = () => events.HasEvents.ShouldBeTrue();
        It should_have_a_count_of_two = () => events.Count.ShouldEqual(2);
        It should_have_the_first_event_at_index_zero = () => events[0].ShouldEqual(event_one);
        It should_have_the_second_event_at_index_one = () => events[1].ShouldEqual(event_two);
    }
}
