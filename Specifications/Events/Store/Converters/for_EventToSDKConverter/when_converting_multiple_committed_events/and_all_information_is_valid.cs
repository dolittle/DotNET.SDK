// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_multiple_committed_events;

public class and_all_information_is_valid : given.a_committed_event_and_a_converter
{
    static string second_content_string;
    static bool second_is_public;
    static EventType second_event_type;
    static EventSourceId second_event_source;
    static DateTimeOffset second_occured;
    static EventLogSequenceNumber second_event_log_sequence_number;
    static DateTimeOffset second_external_event_received;
    static EventLogSequenceNumber second_external_event_log_sequence_number;
    static PbCommittedEvent second_committed_event;

    static object object_from_serializer;
    static object second_object_from_serializer;

    static bool try_result;
    static CommittedEvents converted_events;
    static Exception exception;

    Establish context = () =>
    {
        second_content_string = "wokguakewegheubotidu";
        second_is_public = true;
        second_event_type = new EventType("4edc30a3-be88-4055-a896-ff0f6db49c3b", 1743791927);
        second_event_source = "5d8e9527-0f9d-48bb-b33c-99143dcb96c6";
        second_occured = new DateTimeOffset(2018, 5, 12, 23, 17, 19, TimeSpan.Zero);
        second_event_log_sequence_number = 3448072884;
        second_external_event_received = new DateTimeOffset(2010, 10, 10, 10, 10, 10, TimeSpan.Zero);
        second_external_event_log_sequence_number = 3513533239;

        second_committed_event = new PbCommittedEvent
        {
            External = true,
            Content = second_content_string,
            Public = second_is_public,
            EventType = second_event_type.ToProtobuf(),
            ExecutionContext = execution_context,
            EventSourceId = second_event_source.Value,
            Occurred = Timestamp.FromDateTimeOffset(second_occured),
            EventLogSequenceNumber = second_event_log_sequence_number,
            ExternalEventReceived = Timestamp.FromDateTimeOffset(second_external_event_received),
            ExternalEventLogSequenceNumber = second_external_event_log_sequence_number,
        };

        object_from_serializer = new object();
        SetupDeserializeToReturnObject(content_string, object_from_serializer);
        second_object_from_serializer = new object();
        SetupDeserializeToReturnObject(second_content_string, second_object_from_serializer);
    };

    Because of = () => try_result = converter.TryConvert([committed_event, second_committed_event], out converted_events, out exception);

    It should_return_true = () => try_result.ShouldBeTrue();
    It should_have_no_exception = () => exception.ShouldBeNull();

    It should_create_an_internal_committed_event_for_the_first_event = () => converted_events[0].ShouldBeOfExactType<CommittedEvent>();
    It should_have_the_correct_event_log_sequence_number_for_the_first_event = () => converted_events[0].EventLogSequenceNumber.ShouldEqual(event_log_sequence_number);
    It should_have_the_correct_occurred_for_the_first_event = () => converted_events[0].Occurred.ShouldEqual(occured);
    It should_have_the_correct_event_source_for_the_first_event = () => converted_events[0].EventSource.ShouldEqual(event_source);
    It should_have_the_correct_execution_context_for_the_first_event = () => converted_events[0].ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
    It should_have_the_correct_event_type_for_the_first_event = () => converted_events[0].EventType.ShouldEqual(event_type);
    It should_have_the_correct_content_for_the_first_event = () => converted_events[0].Content.ShouldBeTheSameAs(object_from_serializer);
    It should_have_the_correct_is_public_for_the_first_event = () => converted_events[0].IsPublic.ShouldEqual(is_public);

    It should_create_an_external_committed_event_for_the_second_event = () => converted_events[1].ShouldBeOfExactType<CommittedExternalEvent>();
    It should_have_the_correct_event_log_sequence_number_for_the_second_event = () => converted_events[1].EventLogSequenceNumber.ShouldEqual(second_event_log_sequence_number);
    It should_have_the_correct_occurred_for_the_second_event = () => converted_events[1].Occurred.ShouldEqual(second_occured);
    It should_have_the_correct_event_source_for_the_second_event = () => converted_events[1].EventSource.ShouldEqual(second_event_source);
    It should_have_the_correct_execution_context_for_the_second_event = () => converted_events[1].ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
    It should_have_the_correct_event_type_for_the_second_event = () => converted_events[1].EventType.ShouldEqual(second_event_type);
    It should_have_the_correct_content_for_the_second_event = () => converted_events[1].Content.ShouldBeTheSameAs(second_object_from_serializer);
    It should_have_the_correct_is_public_for_the_second_event = () => converted_events[1].IsPublic.ShouldEqual(second_is_public);

    It should_have_called_the_serializer_with_the_event_types = () => deserialized_event_types.ShouldContainOnly(event_type, second_event_type);
    It should_have_called_the_serializer_with_the_sequence_number = () => deserialized_sequence_numbers.ShouldContainOnly(event_log_sequence_number, second_event_log_sequence_number);
    It should_have_called_the_serializer_with_the_content = () => deserialized_contents.ShouldContainOnly(content_string, second_content_string);
}