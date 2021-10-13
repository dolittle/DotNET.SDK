// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToProtobufConverter.when_converting
{
    public class valid_uncommitted_events : given.a_converter_and_uncommitted_events
    {
        static IReadOnlyList<PbUncommittedEvent> converted_uncommitted_events;
        static Exception exception;
        static bool try_result;

        Establish context = () =>
        {
            SetupSerializeToReturnJSON(content_one, content_as_string_one);
            SetupSerializeToReturnJSON(content_two, content_as_string_two);
        };

        Because of = () => try_result = converter.TryConvert(uncommitted_events, out converted_uncommitted_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();

        It should_return_three_events = () => converted_uncommitted_events.Count.ShouldEqual(3);
        It should_have_called_the_serializer_with_the_three_contents = () => serialized_contents.ShouldContainOnly(content_one, content_two, content_two);

        It should_have_the_same_event_source_for_the_first_event = () => converted_uncommitted_events[0].EventSourceId.ShouldEqual(event_source_one.Value);
        It should_have_the_same_event_type_id_for_the_first_event = () => converted_uncommitted_events[0].EventType.Id.ShouldEqual(event_type_one.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_first_event = () => converted_uncommitted_events[0].EventType.Generation.ShouldEqual(event_type_one.Generation.Value);
        It should_have_content_set_by_the_serializer_on_the_first_event = () => converted_uncommitted_events[0].Content.ShouldEqual(content_as_string_one);
        It should_have_the_same_is_public_for_the_first_event = () => converted_uncommitted_events[0].Public.ShouldEqual(is_public_one);

        It should_have_the_same_event_source_for_the_second_event = () => converted_uncommitted_events[1].EventSourceId.ShouldEqual(event_source_two.Value);
        It should_have_the_same_event_type_id_for_the_second_event = () => converted_uncommitted_events[1].EventType.Id.ShouldEqual(event_type_two.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_second_event = () => converted_uncommitted_events[1].EventType.Generation.ShouldEqual(event_type_two.Generation.Value);
        It should_have_content_set_by_the_serializer_on_the_second_event = () => converted_uncommitted_events[1].Content.ShouldEqual(content_as_string_two);

        It should_have_the_same_is_public_for_the_second_event = () => converted_uncommitted_events[1].Public.ShouldEqual(is_public_two);
        It should_have_the_same_event_source_for_the_third_event = () => converted_uncommitted_events[2].EventSourceId.ShouldEqual(event_source_two.Value);
        It should_have_the_same_event_type_id_for_the_third_event = () => converted_uncommitted_events[2].EventType.Id.ShouldEqual(event_type_two.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_third_event = () => converted_uncommitted_events[2].EventType.Generation.ShouldEqual(event_type_two.Generation.Value);
        It should_have_content_set_by_the_serializer_on_the_third_event = () => converted_uncommitted_events[2].Content.ShouldEqual(content_as_string_two);
        It should_have_the_same_is_public_for_the_third_event = () => converted_uncommitted_events[2].Public.ShouldEqual(is_public_two);
    }
}
