// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using PbUncommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.UncommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToProtobufConverter.when_converting
{
    public class valid_uncommitted_events : given.a_converter_and_uncommitted_events
    {
        static PbUncommittedAggregateEvents converted_uncommitted_events;
        static Exception exception;
        static bool try_result;

        delegate void TryToSerializeCallback(object source, out string jsonString, out Exception error);

        Establish context = () =>
        {
            SetupSerializeToReturnJSON(content_one, content_as_string_one);
            SetupSerializeToReturnJSON(content_two, content_as_string_two);
        };

        Because of = () => try_result = converter.TryConvert(uncommitted_aggregate_events, out converted_uncommitted_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();

        It should_return_three_events = () => converted_uncommitted_events.Events.Count.ShouldEqual(3);
        It should_have_called_the_serializer_with_the_three_contents = () => serialized_contents.ShouldContainOnly(content_one, content_two, content_two);

        It should_the_same_event_source_id = () => converted_uncommitted_events.EventSourceId.ShouldEqual(event_source_id.Value);
        It should_have_the_same_aggregate_root_id = () => converted_uncommitted_events.AggregateRootId.ShouldEqual(aggregate_root_id.ToProtobuf());
        It should_have_the_same_aggregate_root_version = () => converted_uncommitted_events.ExpectedAggregateRootVersion.ShouldEqual(aggregate_root_version.Value);
        It should_have_the_same_event_type_id_for_the_first_event = () => converted_uncommitted_events.Events[0].Artifact.Id.ShouldEqual(event_type_one.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_first_event = () => converted_uncommitted_events.Events[0].Artifact.Generation.ShouldEqual(event_type_one.Generation.Value);
        It should_have_content_set_by_the_serializer_on_the_first_event = () => converted_uncommitted_events.Events[0].Content.ShouldEqual(content_as_string_one);
        It should_have_the_same_is_public_for_the_first_event = () => converted_uncommitted_events.Events[0].Public.ShouldEqual(is_public_one);
        It should_have_the_same_event_type_id_for_the_second_event = () => converted_uncommitted_events.Events[1].Artifact.Id.ShouldEqual(event_type_two.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_second_event = () => converted_uncommitted_events.Events[1].Artifact.Generation.ShouldEqual(event_type_two.Generation.Value);
        It should_have_content_set_by_the_serializer_on_the_second_event = () => converted_uncommitted_events.Events[1].Content.ShouldEqual(content_as_string_two);
        It should_have_the_same_is_public_for_the_second_event = () => converted_uncommitted_events.Events[1].Public.ShouldEqual(is_public_two);
        It should_have_the_same_event_type_id_for_the_third_event = () => converted_uncommitted_events.Events[2].Artifact.Id.ShouldEqual(event_type_two.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_third_event = () => converted_uncommitted_events.Events[2].Artifact.Generation.ShouldEqual(event_type_two.Generation.Value);
        It should_have_content_set_by_the_serializer_on_the_third_event = () => converted_uncommitted_events.Events[2].Content.ShouldEqual(content_as_string_two);
        It should_have_the_same_is_public_for_the_third_event = () => converted_uncommitted_events.Events[2].Public.ShouldEqual(is_public_two);
    }
}
