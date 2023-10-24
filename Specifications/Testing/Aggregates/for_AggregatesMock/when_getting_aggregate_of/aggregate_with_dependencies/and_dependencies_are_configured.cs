// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregatesMock.when_getting_aggregate_of.aggregate_with_dependencies;

class and_dependencies_are_configured : given.aggregates_mock_with_services<AggregateRootWithDependencies>
{
    static AggregateRootWithDependencies.SomeDependencies aggregate_dependecy;
    
    Establish context = () =>
    {
        aggregate_dependecy = new AggregateRootWithDependencies.SomeDependencies(2);
        configure_services = services => services.AddSingleton(aggregate_dependecy);
    };

    Because of = getting_the_aggregate_of;

    It should_get_the_aggregate_of = () => aggregate_of.ShouldBeOfExactType<AggregateOfMock<AggregateRootWithDependencies>>();
    It should_be_able_to_get_the_correct_instance_of_the_aggregate = () =>
    {
        var aggregate =aggregate_of_mock.GetAggregate("some event source"); 
        aggregate.EventSourceId.Value.ShouldEqual("some event source");
        aggregate.TheDependency.ShouldEqual(aggregate_dependecy);
    };
    It should_get_the_same_instance_again = () => get_aggregate_of().ShouldEqual(aggregate_of);
}