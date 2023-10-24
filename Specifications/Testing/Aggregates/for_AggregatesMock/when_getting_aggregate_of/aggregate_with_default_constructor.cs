// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregatesMock.when_getting_aggregate_of;

class aggregate_with_default_constructor : given.aggregates_mock_without_services<AggregateRootWithDefaultConstructor>
{
    Because of = getting_the_aggregate_of;

    It should_get_the_aggregate_of = () => aggregate_of.ShouldBeOfExactType<AggregateOfMock<AggregateRootWithDefaultConstructor>>();
    It should_be_able_to_get_the_correct_instance_of_the_aggregate = () => aggregate_of_mock.GetAggregate("some event source").EventSourceId.Value.ShouldEqual("some event source");
    It should_get_the_same_instance_again = () => get_aggregate_of().ShouldEqual(aggregate_of);
}