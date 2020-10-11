// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_multiple_committed_events
{
    public class and_serializer_fails_for_second_event : given.a_committed_event_and_a_converter
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
        static Exception exception_from_serializer;

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
                Type = second_event_type.ToProtobuf(),
                ExecutionContext = execution_context,
                EventSourceId = second_event_source.ToProtobuf(),
                Occurred = Timestamp.FromDateTimeOffset(second_occured),
                EventLogSequenceNumber = second_event_log_sequence_number,
                ExternalEventReceived = Timestamp.FromDateTimeOffset(second_external_event_received),
                ExternalEventLogSequenceNumber = second_external_event_log_sequence_number,
            };

            object_from_serializer = new object();
            SetupDeserializeToReturnObject(content_string, object_from_serializer);
            exception_from_serializer = new Exception();
            SetupDeserializeToFail(second_content_string, exception_from_serializer);
        };

        Because of = () => try_result = converter.TryConvert(new[] { committed_event, second_committed_event }, out converted_events, out exception);

        It should_return_false = () => try_result.ShouldBeFalse();
        It should_out_default_event = () => converted_events.ShouldEqual(default);
        It should_out_invalid_committed_event_information = () => exception.ShouldBeOfExactType<InvalidCommittedEventInformation>();
        It should_out_invalid_committed_event_information_with_serializer_inner_exception = () => exception.InnerException.ShouldBeTheSameAs(exception_from_serializer);
    }
}
