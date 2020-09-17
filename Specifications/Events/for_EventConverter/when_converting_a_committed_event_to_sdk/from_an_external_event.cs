// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Newtonsoft.Json;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_a_committed_event_to_sdk
{
    public class from_an_external_event : given.a_converter_and_a_protobuf_execution_context
    {
        static an_event content;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Timestamp occured;
        static ulong event_log_sequence_number;
        static Timestamp external_event_received;
        static ulong external_event_log_sequence_number;

        static PbCommittedEvent committed_event;

        static EventType converted_event_type;

        static CommittedEvent converted_committed_event;

        Establish context = () =>
        {
            content = new an_event("hi", 13, true);
            is_public = true;
            event_type = new PbArtifact
            {
                Id = Guid.Parse("81c76d79-88ac-45c7-9e13-7aec35be6483").ToProtobuf(),
                Generation = 553829618,
            };
            event_source = Guid.Parse("9e7abc94-e0c9-4bd1-85e1-5611e7353d95").ToProtobuf();
            occured = Timestamp.FromDateTime(new DateTime(2018, 5, 12, 22, 17, 19, DateTimeKind.Utc));
            event_log_sequence_number = 1469404581;
            external_event_received = Timestamp.FromDateTime(new DateTime(1990, 7, 19, 8, 30, 5, DateTimeKind.Utc));
            external_event_log_sequence_number = 4145723154;

            committed_event = new PbCommittedEvent
            {
                External = true,
                Content = JsonConvert.SerializeObject(content),
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

            event_types.Setup(_ => _.GetTypeFor(converted_event_type)).Returns(typeof(an_event));
        };

        Because of = () => converted_committed_event = converter.ToSDK(committed_event);

        It should_create_an_internal_committed_event = () => converted_committed_event.ShouldBeOfExactType<CommittedExternalEvent>();
        It should_have_asked_the_event_types_for_the_clr_type = () => event_types.Verify(_ => _.GetTypeFor(converted_event_type));
        It should_have_the_correct_event_log_sequence_number = () => converted_committed_event.EventLogSequenceNumber.ShouldEqual((EventLogSequenceNumber)event_log_sequence_number);
        It should_have_the_correct_occurred = () => converted_committed_event.Occurred.ShouldEqual(occured.ToDateTimeOffset());
        It should_have_the_correct_event_source = () => converted_committed_event.EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_the_correct_execution_context = () => converted_committed_event.ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_the_correct_event_type = () => converted_committed_event.EventType.ShouldEqual(converted_event_type);
        It should_have_the_correct_clr_type_of_the_event = () => converted_committed_event.Content.ShouldBeOfExactType<an_event>();
        It should_have_created_a_new_event_instance_with_the_correct_properties = () => (converted_committed_event.Content as an_event).ShouldMatch(_ => _.a_string == content.a_string && _.an_integer == content.an_integer && _.a_bool == content.a_bool);
        It should_have_the_correct_is_public = () => converted_committed_event.IsPublic.ShouldEqual(is_public);
        It should_have_the_correct_external_event_received = () => (converted_committed_event as CommittedExternalEvent).ExternalEventReceived.ShouldEqual(external_event_received.ToDateTimeOffset());
        It should_have_the_correct_external_event_log_sequence_number = () => (converted_committed_event as CommittedExternalEvent).ExternalEventLogSequenceNumber.ShouldEqual((EventLogSequenceNumber)external_event_log_sequence_number);
    }
}