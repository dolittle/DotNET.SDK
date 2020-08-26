// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using given = Dolittle.Events.given;

namespace Dolittle.Events.for_UncommittedAggregateEvents.given
{
    public abstract class an_aggregate_and_two_events : given::Events
    {
        public static EventSourceId event_source_id = Guid.Parse("68b63a42-1736-48b2-ad1e-58dab11e07bf");
        public static Type aggregate_root_type = typeof(aggregate_one);
        public static AggregateRootVersion aggregate_root_version = 1;

        public static CommittedAggregateEvent first_event;
        public static CommittedAggregateEvent second_event;
        public static CommittedAggregateEvent event_with_other_event_source_id;
        public static CommittedAggregateEvent event_with_other_aggregate_root_type;
        public static CommittedAggregateEvent event_with_earlier_event_log_version;
        public static CommittedAggregateEvent event_with_earlier_aggregate_root_version;

        Establish context = () =>
        {
            first_event = new CommittedAggregateEvent(0, DateTimeOffset.Now, event_source_id, aggregate_root_type, 0, execution_context, event_one);

            second_event = new CommittedAggregateEvent(1, DateTimeOffset.Now, event_source_id, aggregate_root_type, 1, execution_context, event_two);

            event_with_other_event_source_id = new CommittedAggregateEvent(2, DateTimeOffset.Now, Guid.Parse("c66f1366-8ba6-46ab-83d2-538fe5712db0"), aggregate_root_type, 2, execution_context, event_two);

            event_with_other_aggregate_root_type = new CommittedAggregateEvent(2, DateTimeOffset.Now, event_source_id, typeof(aggregate_two), 2, execution_context, event_two);

            event_with_earlier_event_log_version = new CommittedAggregateEvent(0, DateTimeOffset.Now, event_source_id, aggregate_root_type, 2, execution_context, event_two);

            event_with_earlier_aggregate_root_version = new CommittedAggregateEvent(2, DateTimeOffset.Now, event_source_id, aggregate_root_type, 0, execution_context, event_two);
        };

        class aggregate_one { }

        class aggregate_two { }
    }
}