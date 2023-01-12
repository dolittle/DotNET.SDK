// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.when_trying_to_get_aggregate;

class and_aggregate_has_been_used : given.an_aggregate_of<StatelessAggregateRoot>
{
    static bool result;
    static StatelessAggregateRoot aggregate;
    static EventSourceId event_source;
    
    Establish context = () =>
    {
        event_source = "the event source";
        use_aggregate_factory(_ => new StatelessAggregateRoot(_));
        aggregate_of.Get(event_source);
    };
    
    Because of = () => result = aggregate_of.TryGetAggregate(event_source, out aggregate);

    It should_return_true = () => result.ShouldBeTrue();
    It should_get_the_correct_aggregate = () => aggregate.EventSourceId.ShouldEqual(event_source);
    It should_invoke_factory_only_once = () => aggregate_factory.Verify(_ => _.Invoke(Moq.It.IsAny<EventSourceId>()), Moq.Times.Once);
    It should_invoke_factory_with_the_correct_event_source = () => aggregate_factory.Verify(_ => _.Invoke(event_source), Moq.Times.Once);
}