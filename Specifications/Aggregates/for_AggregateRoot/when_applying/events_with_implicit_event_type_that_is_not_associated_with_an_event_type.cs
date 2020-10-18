// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_applying
{
    public class events_with_implicit_event_type_that_is_not_associated_with_an_event_type : given.two_aggregate_roots
    {
        static Exception exception;
        Because of = () => exception = Catch.Exception(() => statefull_aggregate_root.Apply(new object()));

        It should_fail_because_event_is_not_associated_to_an_event_type = () => exception.ShouldBeOfExactType<NoEventTypeAssociatedWithType>();
    }
}
