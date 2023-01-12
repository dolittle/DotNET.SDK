// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.when_getting.and_an_operations_is_performed_on_stateful_aggregate;

class and_operation_does_not_fail : given.an_aggregate_of_stateful_aggregate
{
    static EventSourceId event_source;
    static int new_state;

    Establish context = () =>
    {
        event_source = "event source";
        new_state = 3;
    };
    
    Because of = () => aggregate_of.Get(event_source).Perform(_ => _.EventCausingStateChange(new_state)).GetAwaiter().GetResult();

    It should_have_stored_state_on_aggregate = () => aggregate_of.GetAggregate(event_source).TheState.ShouldEqual(new_state);
    It should_after_last_operation_have_the_correct_event = () => aggregate_of.AfterLastOperationOn(event_source).ShouldHaveEvent<EventCausingStateChange>()
        .CountOf(1)
        .AtEnd()
        .Where(_ => _.NewState.ShouldEqual(new_state));
    It should_assert_that_it_the_correct_event = () => aggregate_of.AssertThat(event_source).ShouldHaveEvent<EventCausingStateChange>()
        .CountOf(1)
        .AtEnd()
        .Where(_ => _.NewState.ShouldEqual(new_state));
}