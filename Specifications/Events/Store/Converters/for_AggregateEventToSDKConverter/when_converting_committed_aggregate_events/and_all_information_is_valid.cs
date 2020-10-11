// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToSDKConverter.when_converting_committed_aggregate_events
{
    public class and_all_information_is_valid : given.uncommitted_aggregate_events_and_a_converter
    {
        static object object_from_serializer;

        static bool try_result;
        static CommittedAggregateEvents converted_events;
        static Exception exception;

        Establish context = () =>
        {
            object_from_serializer = new object();
            SetupDeserializeToReturnObject(content_string, object_from_serializer);
        };

        Because of = () => try_result = converter.TryConvert(committed_aggregate_events, out converted_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It should_have_the_correct_event_source = () => converted_events.EventSource.ShouldEqual(event_source);
        It should_have_the_correct_aggregate_root_id = () => converted_events.AggregateRoot.ShouldEqual(aggregate_root_id);
        It should_have_the_correct_aggregate_root_version = () => converted_events.AggregateRootVersion.ShouldEqual(aggregate_root_version);
        It should_have_one_event = () => converted_events.Count.ShouldEqual(1);
        It should_have_one_event_with_the_correct_aggregate_root_id = () => converted_events[0].AggregateRoot.ShouldEqual(aggregate_root_id);
        It should_have_one_event_with_the_correct_aggregate_root_version = () => converted_events[0].AggregateRootVersion.ShouldEqual(aggregate_root_version);
        It should_have_one_event_with_the_correct_occurred = () => converted_events[0].Occurred.ShouldEqual(occured);
        It should_have_one_event_with_the_correct_event_source = () => converted_events[0].EventSource.ShouldEqual(event_source);
        It should_have_one_event_with_the_correct_execution_context = () => converted_events[0].ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_one_event_with_the_correct_event_type = () => converted_events[0].EventType.ShouldEqual(event_type);
        It should_have_called_the_serializer_with_the_event_type = () => deserialized_event_types.ShouldContainOnly(event_type);
        It should_have_called_the_serializer_with_the_sequence_number = () => deserialized_sequence_numbers.ShouldContainOnly(event_log_sequence_number);
        It should_have_called_the_serializer_with_the_content = () => deserialized_contents.ShouldContainOnly(content_string);
        It should_have_one_event_with_the_correct_content = () => converted_events[0].Content.ShouldBeTheSameAs(object_from_serializer);
        It should_have_one_event_with_the_correct_is_public = () => converted_events[0].IsPublic.ShouldEqual(is_public);
    }
}
