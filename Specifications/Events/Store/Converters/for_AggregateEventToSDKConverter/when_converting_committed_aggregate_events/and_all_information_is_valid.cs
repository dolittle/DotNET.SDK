// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedAggregateEvent = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent;
using PbCommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToSDKConverter.when_converting_committed_aggregate_events
{
    public class and_all_information_is_valid : given.a_converter_and_a_protobuf_execution_context
    {
        static string content_string;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Uuid aggregate_root_id;
        static ulong aggregate_root_version;
        static Timestamp occured;
        static ulong event_log_sequence_number;
        static PbCommittedAggregateEvent committed_aggregate_event;
        static PbCommittedAggregateEvents committed_aggregate_events;

        static object object_from_serializer;
        static EventType converted_event_type;

        static bool try_result;
        static CommittedAggregateEvents converted_events;
        static Exception exception;

        Establish context = () =>
        {
            content_string = "ojmikemitethulotlidruewim";
            is_public = false;
            event_type = new PbArtifact
            {
                Id = Guid.Parse("18af5922-9cb2-4a26-bab8-513eb5e00d60").ToProtobuf(),
                Generation = 1589312422,
            };
            event_source = Guid.Parse("2844e1b0-2a8b-4b95-bfd7-69809963bf39").ToProtobuf();
            aggregate_root_id = Guid.Parse("717915c1-bb88-4bec-b1c1-61451c5a6608").ToProtobuf();

            aggregate_root_version = 186206759u;
            occured = Timestamp.FromDateTime(new DateTime(2018, 5, 12, 22, 17, 19, DateTimeKind.Utc));
            event_log_sequence_number = 1581420095;

            committed_aggregate_event = new PbCommittedAggregateEvent
            {
                Content = content_string,
                Public = is_public,
                Type = event_type,
                ExecutionContext = execution_context,
                Occurred = occured,
                EventLogSequenceNumber = event_log_sequence_number,
            };
            committed_aggregate_events = new PbCommittedAggregateEvents
            {
                EventSourceId = event_source,
                AggregateRootId = aggregate_root_id,
                AggregateRootVersion = aggregate_root_version,
                Events = { committed_aggregate_event }
            };

            object_from_serializer = new object();
            SetupDeserializeToReturnObject(content_string, object_from_serializer);

            converted_event_type = new EventType(event_type.Id.To<EventTypeId>(), event_type.Generation);
        };

        Because of = () => try_result = converter.TryConvert(committed_aggregate_events, out converted_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It should_have_the_correct_event_source = () => converted_events.EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_the_correct_aggregate_root_id = () => converted_events.AggregateRoot.ShouldEqual(aggregate_root_id.To<AggregateRootId>());
        It should_have_the_correct_aggregate_root_version = () => converted_events.AggregateRootVersion.Value.ShouldEqual(aggregate_root_version);
        It should_have_one_event = () => converted_events.Count.ShouldEqual(1);
        It should_have_one_event_with_the_correct_aggregate_root_id = () => converted_events[0].AggregateRoot.ShouldEqual(aggregate_root_id.To<AggregateRootId>());
        It should_have_one_event_with_the_correct_aggregate_root_version = () => converted_events[0].AggregateRootVersion.Value.ShouldEqual(aggregate_root_version);
        It should_have_one_event_with_the_correct_occurred = () => converted_events[0].Occurred.ShouldEqual(occured.ToDateTimeOffset());
        It should_have_one_event_with_the_correct_event_source = () => converted_events[0].EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_one_event_with_the_correct_execution_context = () => converted_events[0].ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_one_event_with_the_correct_event_type = () => converted_events[0].EventType.ShouldEqual(converted_event_type);
        It should_have_called_the_serializer_with_the_event_type = () => deserialized_event_types.ShouldContainOnly(converted_event_type);
        It should_have_called_the_serializer_with_the_sequence_number = () => deserialized_sequence_numbers.ShouldContainOnly<EventLogSequenceNumber>(event_log_sequence_number);
        It should_have_called_the_serializer_with_the_content = () => deserialized_contents.ShouldContainOnly(content_string);
        It should_have_one_event_with_the_correct_content = () => converted_events[0].Content.ShouldBeTheSameAs(object_from_serializer);
        It should_have_one_event_with_the_correct_is_public = () => converted_events[0].IsPublic.ShouldEqual(is_public);
    }
}
