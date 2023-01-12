// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregatesMock.when_getting_aggregate_of.aggregate_with_dependencies;

class and_dependencies_are_not_configured : given.aggregates_mock_with_services<AggregateRootWithDependencies>
{
    Because of = getting_the_aggregate_of;

    It should_get_the_aggregate_of = () => aggregate_of.ShouldBeOfExactType<AggregateOfMock<AggregateRootWithDependencies>>();
    It should_fail_when_gettinginstance_of_the_aggregate = () =>
    {
        Catch.Exception(() => aggregate_of_mock.GetAggregate("some event source")).ShouldBeOfExactType<CouldNotCreateAggregateRootInstance>();
    };
    It should_get_the_same_instance_again = () => get_aggregate_of().ShouldEqual(aggregate_of);
}