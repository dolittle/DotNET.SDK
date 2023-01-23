// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.when_getting.and_an_operations_is_performed_on_stateful_aggregate;

class and_second_operation_fails : given.an_aggregate_of_stateful_aggregate
{
    static EventSourceId event_source;
    static int previous_state;
    static Exception failure;

    Establish context = () =>
    {
        event_source = "event source";
        previous_state = 3;
        aggregate_of.GetMock(event_source).PerformSync(_ => _.EventCausingStateChange(previous_state));
    };
    
    Because of = async () => failure = await Catch.ExceptionAsync(() => aggregate_of.Get(event_source).Perform(_ => _.Fail()));

    It should_fail_because_of_operation_failure = () => failure.ShouldBeOfExactType<AggregateRootOperationFailed>();
    It should_have_previous_state_on_aggregate = () => aggregate_of.GetAggregate(event_source).TheState.ShouldEqual(previous_state);
    It should_after_last_operation_have_no_events = () => aggregate_of.AfterLastOperationOn(event_source).ShouldHaveNoEvents();
    It should_assert_that_it_has_the_correct_event = () => aggregate_of.AssertThat(event_source).ShouldHaveEvent<EventCausingStateChange>()
        .CountOf(1)
        .AtEnd()
        .Where(_ => _.NewState.ShouldEqual(previous_state));
}