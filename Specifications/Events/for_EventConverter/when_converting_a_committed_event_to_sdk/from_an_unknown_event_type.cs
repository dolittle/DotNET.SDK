// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_a_committed_event_to_sdk
{
    public class from_an_unknown_event_type : given.a_converter_and_a_protobuf_execution_context
    {
        static an_event content;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Timestamp occured;
        static ulong event_log_sequence_number;

        static PbCommittedEvent committed_event;

        static EventType converted_event_type;

        static CommittedEvent converted_committed_event;

        Establish context = () =>
        {
            content = new an_event("juhejupujgos", 13, true);
            is_public = false;
            event_type = new PbArtifact
            {
                Id = Guid.Parse("e0251ba2-3ce5-4fe2-8989-86f307b2cd5e").ToProtobuf(),
                Generation = 2,
            };
            event_source = Guid.Parse("6b3a53ed-e307-4d10-b33d-5cd6515b831d").ToProtobuf();
            occured = Timestamp.FromDateTime(new DateTime(2018, 5, 12, 22, 17, 19, DateTimeKind.Utc));
            event_log_sequence_number = 3117182396;

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

            event_types.Setup(_ => _.HasTypeFor(converted_event_type)).Returns(false);
            event_types.Setup(_ => _.GetTypeFor(converted_event_type)).Throws(new UnknownType(converted_event_type));
        };

        Because of = () => converted_committed_event = converter.ToSDK(committed_event);

        It should_create_an_internal_committed_event = () => converted_committed_event.ShouldBeOfExactType<CommittedEvent>();
        It should_have_asked_the_event_types_if_it_has_type_for = () => event_types.Verify(_ => _.HasTypeFor(converted_event_type));
        It should_not_ask_the_event_types_for_anything_else = () => event_types.VerifyNoOtherCalls();
        It should_have_the_correct_event_log_sequence_number = () => converted_committed_event.EventLogSequenceNumber.ShouldEqual((EventLogSequenceNumber)event_log_sequence_number);
        It should_have_the_correct_occurred = () => converted_committed_event.Occurred.ShouldEqual(occured.ToDateTimeOffset());
        It should_have_the_correct_event_source = () => converted_committed_event.EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_the_correct_execution_context = () => converted_committed_event.ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_the_correct_event_type = () => converted_committed_event.EventType.ShouldEqual(converted_event_type);
        It should_have_the_correct_type_of_the_event = () => converted_committed_event.Content.ShouldBeOfExactType<JObject>();
        It should_have_created_a_new_event_instance_with_the_correct_properties = () => (converted_committed_event.Content as JObject).ShouldMatch(_ => _.Value<string>("a_string") == content.a_string && _.Value<int>("an_integer") == content.an_integer && _.Value<bool>("a_bool") == content.a_bool);
        It should_have_the_correct_is_public = () => converted_committed_event.IsPublic.ShouldEqual(is_public);
    }
}
