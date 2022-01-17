// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Builders.for_EventBuilder.given;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Builders.for_EventBuilder.when_building_event.that_is_associated_with_an_event_type;

public class and_not_specifying_event_type : given.all_dependencies
{
    static (object content, EventType eventType, EventSourceId eventSourceId, bool isPublic) result;

    Because of = () => result = event_builder.Build(event_types.Object);

    It should_get_event_type_from_event_types = () => event_types.Verify(_ => _.GetFor(typeof(some_event_type)), Moq.Times.Once);
    It should_return_the_correct_event_content = () => result.content.ShouldEqual(@event);
    It should_return_the_correct_event_type = () => result.eventType.ShouldEqual(event_type);
    It should_return_the_correct_event_source_id = () => result.eventSourceId.ShouldEqual(event_source_id);
    It should_return_the_correct_is_public_value = () => result.isPublic.ShouldEqual(is_public);
}