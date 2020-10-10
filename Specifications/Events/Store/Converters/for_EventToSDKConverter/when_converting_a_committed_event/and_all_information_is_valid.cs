// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_a_committed_event
{
    public class and_all_information_is_valid : given.a_converter_and_a_protobuf_execution_context
    {
        static string content_string;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Timestamp occured;
        static ulong event_log_sequence_number;
        static PbCommittedEvent committed_event;

        static object object_from_serializer;
        static EventType converted_event_type;

        static bool try_result;
        static CommittedEvent converted_event;
        static Exception exception;

        delegate void TryToDeserializeCallback(string source, EventType eventType, out object content, out Exception error);

        Establish context = () =>
        {
            content_string = "medowfecavremru";
            is_public = false;
            event_type = new PbArtifact
            {
                Id = Guid.Parse("926a967f-3833-47ef-82af-95612b826015").ToProtobuf(),
                Generation = 14,
            };
            event_source = Guid.Parse("d5886138-f29b-4684-bf9b-0e5e13894926").ToProtobuf();
            occured = Timestamp.FromDateTime(new DateTime(2018, 5, 12, 22, 17, 19, DateTimeKind.Utc));
            event_log_sequence_number = 3448072883;

            committed_event = new PbCommittedEvent
            {
                External = false,
                Content = content_string,
                Public = is_public,
                Type = event_type,
                ExecutionContext = execution_context,
                EventSourceId = event_source,
                Occurred = occured,
                EventLogSequenceNumber = event_log_sequence_number,
            };

            object_from_serializer = new object();
            SetupSerializerToReturnObject(content_string, object_from_serializer);

            converted_event_type = new EventType(event_type.Id.To<EventTypeId>(), event_type.Generation);
        };

        Because of = () => try_result = converter.TryConvert(committed_event, out converted_event, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It should_create_an_internal_committed_event = () => converted_event.ShouldBeOfExactType<CommittedEvent>();
        It should_have_the_correct_event_log_sequence_number = () => converted_event.EventLogSequenceNumber.ShouldEqual<EventLogSequenceNumber>(event_log_sequence_number);
        It should_have_the_correct_occurred = () => converted_event.Occurred.ShouldEqual(occured.ToDateTimeOffset());
        It should_have_the_correct_event_source = () => converted_event.EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_the_correct_execution_context = () => converted_event.ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_the_correct_event_type = () => converted_event.EventType.ShouldEqual(converted_event_type);
        It should_have_called_the_serializer_with_the_event_type = () => deserialized_event_types.ShouldContainOnly(converted_event_type);
        It should_have_called_the_serializer_with_the_sequence_number = () => deserialized_sequence_numbers.ShouldContainOnly<EventLogSequenceNumber>(event_log_sequence_number);
        It should_have_called_the_serializer_with_the_content = () => deserialized_contents.ShouldContainOnly(content_string);
        It should_have_the_correct_content = () => converted_event.Content.ShouldBeTheSameAs(object_from_serializer);
        It should_have_the_correct_is_public = () => converted_event.IsPublic.ShouldEqual(is_public);
    }
}
