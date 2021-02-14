// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_applying
{
    public class events_with_event_type_id : given.two_aggregate_roots
    {
        static IEnumerable<AppliedEvent> applied_events;
        Because of = () =>
        {
            statefull_aggregate_root.Apply(first_event, event_type_for_an_event.Id);
            statefull_aggregate_root.Apply(second_event, event_type_for_an_event.Id);
            statefull_aggregate_root.Apply(third_event, event_type_for_another_event.Id);
            applied_events = statefull_aggregate_root.AppliedEvents;
        };

        It should_be_at_version_three = () => statefull_aggregate_root.Version.ShouldEqual<AggregateRootVersion>(3);
        It should_handle_an_event_two_times = () => statefull_aggregate_root.AnEventOnCalled.ShouldEqual(2);
        It should_handle_another_event_one_time = () => statefull_aggregate_root.AnotherEventOnCalled.ShouldEqual(1);

        It should_have_three_events = () => applied_events.Count().ShouldEqual(3);
        It should_have_event_one_at_index_zero = () => applied_event(0).Event.ShouldEqual(first_event);
        It should_have_event_two_at_index_one = () => applied_event(1).Event.ShouldEqual(second_event);
        It should_have_event_three_at_index_two = () => applied_event(2).Event.ShouldEqual(third_event);

        static AppliedEvent applied_event(int pos)
            => applied_events.ToList()[pos];
    }
}
