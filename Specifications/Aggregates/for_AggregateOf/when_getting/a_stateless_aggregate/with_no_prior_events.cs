// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateOf.when_getting.a_stateless_aggregate
{
    public class with_no_prior_events : given.all_dependencies
    {
        static readonly EventSourceId event_source = "819e2d1a-b7a3-42dd-af9b-1fa7f89a1aac";
        static readonly AggregateRootId aggregate_root = new Aggregates.given.StatelessAggregateRoot(event_source).GetAggregateRootId();
        static IAggregateOf<Aggregates.given.StatelessAggregateRoot> aggregate_of;
        static IAggregateRootOperations<Aggregates.given.StatelessAggregateRoot> result;

        Establish context = () =>
        {
            SetupCommittedEvents(aggregate_root, event_source);
            aggregate_of = new AggregateOf<Aggregates.given.StatelessAggregateRoot>(event_store.Object, event_types.Object, logger_factory);
        };

        Because of = () => result = aggregate_of.Get(event_source);

        It should_get_aggregate = () => result.ShouldNotBeNull();
    }
}
