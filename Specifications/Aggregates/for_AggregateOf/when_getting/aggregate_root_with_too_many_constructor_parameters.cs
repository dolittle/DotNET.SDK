// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateOf.when_getting
{
    public class aggregate_root_with_too_many_constructor_parameters : given.all_dependencies
    {
        static readonly EventSourceId event_source = "819e2d1a-b7a3-42dd-af9b-1fa7f89a1aac";
        static readonly AggregateRootId aggregate_root = new given.AggregateRootWithTooManyConstructorParameters("79235c32-d7a8-4338-ba8b-2109b19c7805", "5b08a724-4d29-4242-b3e9-2aa664563450").GetAggregateRootId();
        static IAggregateOf<given.AggregateRootWithTooManyConstructorParameters> aggregate_of;
        static Exception exception;

        Establish context = () =>
            aggregate_of = new AggregateOf<given.AggregateRootWithTooManyConstructorParameters>(event_store.Object, event_types.Object, logger_factory);

        Because of = () => exception = Catch.Exception(() => aggregate_of.Get(event_source));

        It should_fail_because_it_could_not_get_aggregate_root = () => exception.ShouldBeOfExactType<CouldNotGetAggregateRoot>();
    }
}
