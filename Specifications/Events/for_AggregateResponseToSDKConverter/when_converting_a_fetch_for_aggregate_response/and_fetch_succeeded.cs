// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;
using PbCommittedAggregateEvent = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent;
using PbCommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents;
using PbFetchForAggregateResponse = Dolittle.Runtime.Events.Contracts.FetchForAggregateResponse;

namespace Dolittle.SDK.Events.for_AggregateResponseToSDKConverter.when_converting_a_fetch_for_aggregate_response
{
    public class and_fetch_succeeded : given.a_converter_and_a_protobuf_execution_context
    {
        static string content_string;
        static bool is_public;
        static PbArtifact event_type;
        static Uuid event_source;
        static Uuid aggregate_root_id;
        static ulong aggregate_root_version;
        static Timestamp occured;
        static ulong event_log_sequence_number;
        static EventType converted_event_type;
        static PbCommittedAggregateEvent committed_aggregate_event;
        static PbCommittedAggregateEvents committed_aggregate_events;
        static PbFetchForAggregateResponse fetch_for_aggregate_events_response;
        static CommittedAggregateEvents committed_events;
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
            aggregate_root_id = Guid.Parse("561c0d14-51b9-4efd-9b8e-b84eb278e0ab").ToProtobuf();

            // add tests for dealing with the version thingy
            aggregate_root_version = 123u;
            occured = Timestamp.FromDateTime(new DateTime(2018, 5, 12, 22, 17, 19, DateTimeKind.Utc));
            event_log_sequence_number = 3448072883;

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

            fetch_for_aggregate_events_response = new PbFetchForAggregateResponse
            {
                Failure = null,
                Events = committed_aggregate_events
            };

            converted_event_type = new EventType(event_type.Id.To<EventTypeId>(), event_type.Generation);

            serializer
                .Setup(_ => _.TryToDeserialize(committed_aggregate_event.Content, Moq.It.IsAny<EventType>(), out Moq.It.Ref<object>.IsAny, out Moq.It.Ref<Exception>.IsAny))
                .Callback(new TryToDeserializeCallback((string _, EventType eventType, out object content, out Exception error) =>
                {
                    content = content_string;
                    error = null;
                }))
                .Returns(true);
        };

        Because of = () => try_result = converter.TryToSDK(fetch_for_aggregate_events_response, out committed_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It should_have_one_event = () => committed_events.Count.ShouldEqual(1);
        It should_create_an_internal_committed_event = () => committed_events[0].ShouldBeOfExactType<CommittedAggregateEvent>();
        It should_have_the_correct_event_log_sequence_number = () => committed_events[0].EventLogSequenceNumber.ShouldEqual((EventLogSequenceNumber)event_log_sequence_number);
        It should_have_the_correct_occurred = () => committed_events[0].Occurred.ShouldEqual(occured.ToDateTimeOffset());
        It should_have_the_correct_event_source = () => committed_events.EventSource.ShouldEqual(event_source.To<EventSourceId>());
        It should_have_the_correct_aggregate_root_id = () => committed_events.AggregateRootId.ShouldEqual(aggregate_root_id.To<AggregateRootId>());
        It should_have_the_correct_aggregate_root_version = () => committed_events.AggregateRootVersion.Value.ShouldEqual(aggregate_root_version);
        It should_have_the_correct_execution_context = () => committed_events[0].ExecutionContext.ShouldEqual(execution_context.ToExecutionContext());
        It should_have_the_correct_event_type = () => committed_events[0].EventType.ShouldEqual(converted_event_type);
        It should_have_the_correct_is_public = () => committed_events[0].IsPublic.ShouldEqual(is_public);
        It should_have_called_the_serializer_with_the_content = () =>
            serializer.Verify(_ => _.TryToDeserialize(fetch_for_aggregate_events_response.Events.Events[0].Content, Moq.It.IsAny<EventType>(), out Moq.It.Ref<object>.IsAny, out Moq.It.Ref<Exception>.IsAny), Times.Once());
    }
}
