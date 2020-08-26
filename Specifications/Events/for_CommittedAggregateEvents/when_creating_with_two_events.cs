﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Events.for_CommittedAggregateEvents
{
    public class when_creating_with_two_events : given.an_aggregate_instance_and_some_committed_events
    {
        static CommittedAggregateEvents events;

        Because of = () => events = new CommittedAggregateEvents(event_source_id, aggregate_root_type, new CommittedAggregateEvent[] { first_event, second_event });

        It should_have_events = () => events.HasEvents.ShouldBeTrue();
        It should_have_a_count_of_two = () => events.Count.ShouldEqual(2);
        It should_have_the_first_event_at_index_zero = () => events[0].ShouldEqual(first_event);
        It should_have_the_first_event_at_index_one = () => events[1].ShouldEqual(second_event);
    }
}
