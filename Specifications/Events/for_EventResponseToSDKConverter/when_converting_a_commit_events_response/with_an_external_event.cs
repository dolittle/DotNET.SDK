// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.for_EventResponseToSDKConverter.when_converting_a_commit_events_response
{
    public class with_an_external_event : given.a_converter_and_a_protobuf_execution_context
    {
        static string content_string;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Timestamp occured;
        static ulong event_log_sequence_number;
        static Timestamp external_event_received;
        static ulong external_event_log_sequence_number;
        static PbCommittedEvent committed_event;
        static CommitEventsResponse commit_events_response;
        static CommittedEvents committed_events;
        static EventType converted_event_type;
        static Exception exception;
        static bool try_result;

        delegate void TryToDeserializeCallback(string source, EventType eventType, out object content, out Exception error);

        Establish context = () =>
        {
            content_string = "ima a content string hahahahahaah xDxDD1!!11";
            is_public = false;
            event_type = new PbArtifact
            {
                Id = Guid.Parse("926a967f-3833-47ef-82af-95612b826015").ToProtobuf(),
                Generation = 14,
            };
            event_source = Guid.Parse("d5886138-f29b-4684-bf9b-0e5e13894926").ToProtobuf();
            occured = Timestamp.FromDateTime(new DateTime(2018, 5, 12, 22, 17, 19, DateTimeKind.Utc));
            event_log_sequence_number = 3448072883;
            external_event_received = Timestamp.FromDateTime(new DateTime(1990, 7, 19, 8, 30, 5, DateTimeKind.Utc));
            external_event_log_sequence_number = 4145723154;

            committed_event = new PbCommittedEvent
            {
                External = true,
                Content = content_string,
                Public = is_public,
                Type = event_type,
                ExecutionContext = execution_context,
                EventSourceId = event_source,
                Occurred = occured,
                EventLogSequenceNumber = event_log_sequence_number,
                ExternalEventReceived = external_event_received,
                ExternalEventLogSequenceNumber = external_event_log_sequence_number,
            };

            converted_event_type = new EventType(event_type.Id.To<EventTypeId>(), event_type.Generation);

            commit_events_response = new CommitEventsResponse
            {
                Failure = null,
                Events = { committed_event }
            };

            serializer
                .Setup(_ => _.TryToDeserialize(committed_event.Content, Moq.It.IsAny<EventType>(), out Moq.It.Ref<object>.IsAny, out Moq.It.Ref<Exception>.IsAny))
                .Callback(new TryToDeserializeCallback((string _, EventType eventType, out object content, out Exception error) =>
                {
                    content = content_string;
                    error = null;
                }))
                .Returns(true);
        };

        Because of = () => try_result = converter.TryToSDK(commit_events_response, out committed_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It should_have_one_event = () => committed_events.Count.ShouldEqual(1);
        It should_create_an_internal_committed_event = () => committed_events[0].ShouldBeOfExactType<CommittedEvent>();
        It should_have_the_correct_event_log_sequence_number = () => committed_events[0].EventLogSequenceNumber.ShouldEqual((EventLogSequenceNumber)event_log_sequence_number);
        It should_have_the_correct_occurred = () => committed_events[0].Occurred.ShouldEqual(occured.ToDateTimeOffset());
        It should_have_the_correct_event_source = () => committed_events[0].EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_the_correct_execution_context = () => committed_events[0].ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_the_correct_event_type = () => committed_events[0].EventType.ShouldEqual(converted_event_type);
        It should_have_called_the_serializer_with_the_content = () =>
            serializer.Verify(_ => _.TryToDeserialize(commit_events_response.Events[0].Content, Moq.It.IsAny<EventType>(), out Moq.It.Ref<object>.IsAny, out Moq.It.Ref<Exception>.IsAny), Times.Once());

        It should_have_the_correct_is_public = () => committed_events[0].IsPublic.ShouldEqual(is_public);
        It should_have_the_correct_external_event_received = () => (committed_events[0] as CommittedExternalEvent).ExternalEventReceived.ShouldEqual(external_event_received.ToDateTimeOffset());
        It should_have_the_correct_external_event_log_sequence_number = () => (committed_events[0] as CommittedExternalEvent).ExternalEventLogSequenceNumber.ShouldEqual((EventLogSequenceNumber)external_event_log_sequence_number);
    }
}
