// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Newtonsoft.Json;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_a_commit_events_response_to_sdk
{
    public class and_commit_succeeded : a_converter_and_a_protobuf_execution_context
    {
        static an_event content;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Timestamp occured;
        static ulong event_log_sequence_number;
        static PbCommittedEvent committed_event;
        static CommitEventsResponse commit_events_response;
        static CommitEventsResult commit_events_result;
        static EventType converted_event_type;

        Establish context = () =>
        {
            content = new an_event("deadbeef", 13, true);
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
                Content = JsonConvert.SerializeObject(content),
                Public = is_public,
                Type = event_type,
                ExecutionContext = execution_context,
                EventSourceId = event_source,
                Occurred = occured,
                EventLogSequenceNumber = event_log_sequence_number,
            };

            converted_event_type = new EventType(event_type.Id.To<EventTypeId>(), event_type.Generation);
            event_types.Setup(_ => _.HasTypeFor(converted_event_type)).Returns(true);
            event_types.Setup(_ => _.GetTypeFor(converted_event_type)).Returns(typeof(an_event));

            commit_events_response = new CommitEventsResponse
            {
                Failure = null,
                Events = { committed_event }
            };
        };

        Because of = () => commit_events_result = converter.ToSDK(commit_events_response);

        It should_not_have_failed = () => commit_events_result.Failed.ShouldBeFalse();
        It should_not_have_set_a_failure = () => commit_events_result.Failure.ShouldBeNull();
        It should_have_one_event = () => commit_events_result.Events.Count.ShouldEqual(1);
        It should_create_an_internal_committed_event = () => commit_events_result.Events[0].ShouldBeOfExactType<CommittedEvent>();
        It should_have_asked_the_event_types_if_it_has_type_for = () => event_types.Verify(_ => _.HasTypeFor(converted_event_type));
        It should_have_asked_the_event_types_for_the_type = () => event_types.Verify(_ => _.GetTypeFor(converted_event_type));
        It should_not_ask_the_event_types_for_anything_else = () => event_types.VerifyNoOtherCalls();
        It should_have_the_correct_event_log_sequence_number = () => commit_events_result.Events[0].EventLogSequenceNumber.ShouldEqual((EventLogSequenceNumber)event_log_sequence_number);
        It should_have_the_correct_occurred = () => commit_events_result.Events[0].Occurred.ShouldEqual(occured.ToDateTimeOffset());
        It should_have_the_correct_event_source = () => commit_events_result.Events[0].EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_the_correct_execution_context = () => commit_events_result.Events[0].ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_the_correct_event_type = () => commit_events_result.Events[0].EventType.ShouldEqual(converted_event_type);
        It should_have_the_correct_type_of_the_event = () => commit_events_result.Events[0].Content.ShouldBeOfExactType<an_event>();
        It should_have_created_a_new_event_instance_with_the_correct_properties = () => (commit_events_result.Events[0].Content as an_event).ShouldMatch(_ => _.a_string == content.a_string && _.an_integer == content.an_integer && _.a_bool == content.a_bool);
        It should_have_the_correct_is_public = () => commit_events_result.Events[0].IsPublic.ShouldEqual(is_public);
    }
}
