// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_applying
{
    public class events_with_correct_event_type : given.two_aggregate_roots
    {
        static IEnumerable<UncommittedAggregateEvent> uncommitted_events;
        Because of = () =>
        {
            statefull_aggregate_root.Apply(first_event, event_type_for_an_event);
            statefull_aggregate_root.Apply(second_event, event_type_for_an_event);
            statefull_aggregate_root.Apply(third_event, event_type_for_another_event);
            uncommitted_events = statefull_aggregate_root.UncommittedEvents;
        };

        It should_be_at_version_three = () => statefull_aggregate_root.Version.ShouldEqual<AggregateRootVersion>(3);
        It should_handle_an_event_two_times = () => statefull_aggregate_root.AnEventOnCalled.ShouldEqual(2);
        It should_handle_another_event_one_time = () => statefull_aggregate_root.AnotherEventOnCalled.ShouldEqual(1);
        It should_have_three_events = () => uncommitted_events.Count().ShouldEqual(3);
        It should_have_event_one_at_index_zero = () => uncommitted_event(0).Content.ShouldEqual(first_event);
        It should_have_event_two_at_index_one = () => uncommitted_event(1).Content.ShouldEqual(second_event);
        It should_have_event_three_at_index_two = () => uncommitted_event(2).Content.ShouldEqual(third_event);

        static UncommittedAggregateEvent uncommitted_event(int pos)
            => uncommitted_events.ToList()[pos];
    }
}
