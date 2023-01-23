// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.when_getting;

class multiple_operations : given.an_aggregate_of<StatelessAggregateRoot>
{
    static IAggregateRootOperations<StatelessAggregateRoot> first_operation;
    static IAggregateRootOperations<StatelessAggregateRoot> second_operation;
    static EventSourceId event_source;
    
    Establish context = () =>
    {
        event_source = "the event source";
        use_aggregate_factory(_ => new StatelessAggregateRoot(_));
    };
    
    Because of = () =>
    {
        first_operation = aggregate_of.Get(event_source);
        second_operation = aggregate_of.Get(event_source);
    };

    It should_get_the_first_opperation = () => first_operation.ShouldNotBeNull();
    It should_get_the_second_opperation = () => second_operation.ShouldNotBeNull();
    It should_get_different_operations = () => first_operation.ShouldNotBeTheSameAs(second_operation);
    It should_invoke_factory_only_once = () => aggregate_factory.Verify(_ => _.Invoke(Moq.It.IsAny<EventSourceId>()), Moq.Times.Once);
    It should_invoke_factory_with_the_correct_event_source = () => aggregate_factory.Verify(_ => _.Invoke(event_source), Moq.Times.Once);
}