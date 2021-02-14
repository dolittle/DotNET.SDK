// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateOf.when_getting
{
    public class aggregate_root_with_no_constructor_parameters : given.all_dependencies
    {
        static readonly EventSourceId event_source = "819e2d1a-b7a3-42dd-af9b-1fa7f89a1aac";
        static readonly AggregateRootId aggregate_root = new given.AggregateRootWithNoConstructorParameters().GetAggregateRootId();
        static IAggregateOf<given.AggregateRootWithNoConstructorParameters> aggregate_of;
        static Exception exception;

        Establish context = () =>
            aggregate_of = new AggregateOf<given.AggregateRootWithNoConstructorParameters>(event_store.Object, event_types.Object, logger_factory);

        Because of = () => exception = Catch.Exception(() => aggregate_of.Get(event_source));

        It should_fail_because_it_could_not_get_aggregate_root = () => exception.ShouldBeOfExactType<CouldNotGetAggregateRoot>();
    }
}
