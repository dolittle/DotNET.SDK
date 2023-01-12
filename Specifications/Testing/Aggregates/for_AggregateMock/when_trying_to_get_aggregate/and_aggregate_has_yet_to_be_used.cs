// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.when_trying_to_get_aggregate;

class and_aggregate_has_yet_to_be_used : given.an_aggregate_of<StatelessAggregateRoot>
{
    static bool result;
    static StatelessAggregateRoot aggregate;

    Establish context = () =>
    {
        use_aggregate_factory(_ => new StatelessAggregateRoot(_));
    };
    
    Because of = () => result = aggregate_of.TryGetAggregate("an event source", out aggregate);

    It should_return_false = () => result.ShouldBeFalse();
    It should_not_output_aggregate = () => aggregate.ShouldBeNull();
    It should_not_invoke_factory = () => aggregate_factory.Verify(_ => _.Invoke(Moq.It.IsAny<EventSourceId>()), Moq.Times.Never);
}