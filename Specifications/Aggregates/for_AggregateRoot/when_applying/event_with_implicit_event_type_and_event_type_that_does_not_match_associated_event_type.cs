// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_applying
{
    public class event_with_implicit_event_type_and_event_type_that_does_not_match_associated_event_type : given.two_aggregate_roots
    {
        static Exception exception;
        Because of = () => exception = Catch.Exception(() => statefull_aggregate_root.Apply(first_event, new EventType("b753ef0d-68d0-4294-ad8e-aaa5b14f0cd1")));

        It should_fail_because_event_is_not_associated_to_an_event_type = () => exception.ShouldBeOfExactType<ProvidedEventTypeDoesNotMatchEventTypeFromAttribute>();
    }
}
