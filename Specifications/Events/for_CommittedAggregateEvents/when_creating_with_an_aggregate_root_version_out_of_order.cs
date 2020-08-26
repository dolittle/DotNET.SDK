﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Events.for_CommittedAggregateEvents
{
    public class when_creating_with_an_aggregate_root_version_out_of_order : given.an_aggregate_instance_and_some_committed_events
    {
        static CommittedAggregateEvents events;
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => events = new CommittedAggregateEvents(event_source_id, aggregate_root_type, new CommittedAggregateEvent[] { first_event, second_event, event_with_earlier_aggregate_root_version }));

        It should_not_be_created = () => events.ShouldBeNull();
        It should_throw_an_exception = () => exception.ShouldBeOfExactType<AggregateRootVersionIsOutOfOrder>();
    }
}
