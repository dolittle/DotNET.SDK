// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.when_getting.and_an_operations_is_performed_on_stateful_aggregate;

class two_operations_that_modify_state : given.an_aggregate_of_stateful_aggregate
{
    static EventSourceId event_source;
    static int first_state, final_state;

    Establish context = () =>
    {
        event_source = "event source";
        (first_state, final_state) = (3, 2);
        aggregate_of.GetMock(event_source).PerformSync(_ => _.EventCausingStateChange(first_state));
    };
    
    Because of = () => aggregate_of.Get(event_source).Perform(_ => _.EventCausingStateChange(final_state)).GetAwaiter().GetResult();

    It should_have_stored_final_on_aggregate = () => aggregate_of.GetAggregate(event_source).TheState.ShouldEqual(final_state);
    It should_after_last_operation_have_the_correct_event = () => aggregate_of.AfterLastOperationOn(event_source).ShouldHaveEvent<EventCausingStateChange>()
        .CountOf(1)
        .AtEnd()
        .AndThat(_ => _.NewState.ShouldEqual(final_state));
    It should_assert_that_it_the_correct_first_event = () => aggregate_of.AssertThat(event_source).ShouldHaveEvent<EventCausingStateChange>()
        .CountOf(2)
        .First()
        .AndThat(_ => _.NewState.ShouldEqual(first_state));
    It should_assert_that_it_the_correct_last_event = () => aggregate_of.AssertThat(event_source).ShouldHaveEvent<EventCausingStateChange>()
        .CountOf(2)
        .Last()
        .AndThat(_ => _.NewState.ShouldEqual(final_state));
}