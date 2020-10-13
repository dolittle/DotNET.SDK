// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events.Builders.for_EventBuilder.given;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Builders.for_EventBuilder.when_building_event.that_is_associated_with_an_event_type
{
    public class and_specifying_a_non_matching_event_type : given.all_dependencies
    {
        static Exception exception;

        Establish conext = () => event_builder.WithEventType(new EventType("baaf75f1-d1cb-4622-a86d-d0bd3a09e1e5"));
        Because of = () => exception = Catch.Exception(() => event_builder.Build(event_types.Object));

        It should_get_event_type_from_event_types = () => event_types.Verify(_ => _.GetFor(typeof(some_event_type)), Moq.Times.Once);
        It should_fail_because_configured_event_type_does_not_match_associated_event_type = () => exception.ShouldBeOfExactType<ConfiguredEventTypeDoesNotMatchAssociatedEventType>();
    }
}