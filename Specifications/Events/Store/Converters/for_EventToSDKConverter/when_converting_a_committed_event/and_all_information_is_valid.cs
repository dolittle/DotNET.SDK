// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_a_committed_event;

public class and_all_information_is_valid : given.a_committed_event_and_a_converter
{
    static object object_from_serializer;

    static bool try_result;
    static CommittedEvent converted_event;
    static Exception exception;

    Establish context = () =>
    {
        object_from_serializer = new object();
        SetupDeserializeToReturnObject(content_string, object_from_serializer);
    };

    Because of = () => try_result = converter.TryConvert(committed_event, out converted_event, out exception);

    It should_return_true = () => try_result.ShouldBeTrue();
    It should_have_no_exception = () => exception.ShouldBeNull();
    It should_create_an_internal_committed_event = () => converted_event.ShouldBeOfExactType<CommittedEvent>();
    It should_have_the_correct_event_log_sequence_number = () => converted_event.EventLogSequenceNumber.ShouldEqual(event_log_sequence_number);
    It should_have_the_correct_occurred = () => converted_event.Occurred.ShouldEqual(occured);
    It should_have_the_correct_event_source = () => converted_event.EventSource.ShouldEqual(event_source);
    It should_have_the_correct_execution_context = () => converted_event.ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
    It should_have_the_correct_event_type = () => converted_event.EventType.ShouldEqual(event_type);
    It should_have_called_the_serializer_with_the_event_type = () => deserialized_event_types.ShouldContainOnly(event_type);
    It should_have_called_the_serializer_with_the_sequence_number = () => deserialized_sequence_numbers.ShouldContainOnly(event_log_sequence_number);
    It should_have_called_the_serializer_with_the_content = () => deserialized_contents.ShouldContainOnly(content_string);
    It should_have_the_correct_content = () => converted_event.Content.ShouldBeTheSameAs(object_from_serializer);
    It should_have_the_correct_is_public = () => converted_event.IsPublic.ShouldEqual(is_public);
}