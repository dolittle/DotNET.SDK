// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Newtonsoft.Json;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_a_commit_events_response_to_sdk
{
    public class and_commit_failed : a_converter_and_a_protobuf_execution_context
    {
        static an_event content;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Timestamp occured;
        static ulong event_log_sequence_number;
        static PbCommittedEvent committed_event;
        static Uuid failure_id;
        static string failure_reason;
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

            failure_id = Guid.Parse("59a58bd1-6fdb-4b21-aafe-9f7fc111bddd").ToProtobuf();
            failure_reason = "out of tacos";
            commit_events_response = new CommitEventsResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                    {
                        Id = failure_id,
                        Reason = failure_reason
                    },
                Events = { committed_event }
            };
        };

        Because of = () => commit_events_result = converter.ToSDK(commit_events_response);

        It should_have_failed = () => commit_events_result.Failed.ShouldBeTrue();
        It should_have_the_same_failure_id = () => commit_events_result.Failure.Id.ShouldEqual(commit_events_response.Failure.Id.To<FailureId>());
        It should_have_the_same_failure_reason = () => commit_events_result.Failure.Reason.Value.ShouldEqual(commit_events_response.Failure.Reason);
        It should_have_events_set_as_null = () => commit_events_result.Events.ShouldBeNull();
        It should_not_ask_the_event_types_for_anything_else = () => event_types.VerifyNoOtherCalls();
    }
}
