// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbCommittedAggregateEvent = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent;
using PbCommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents;
#pragma warning disable CS0612

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToSDKConverter.when_converting_committed_aggregate_events;

public class and_execution_context_is_null : given.committed_aggregate_events_and_a_converter
{
    static object object_from_serializer;

    static bool try_result;
    static CommittedAggregateEvents converted_events;
    static Exception exception;

    Establish context = () =>
    {
        committed_aggregate_event = new PbCommittedAggregateEvent
        {
            Content = content_string,
            Public = is_public,
            EventType = event_type.ToProtobuf(),
            ExecutionContext = null,
            Occurred = Timestamp.FromDateTimeOffset(occured),
            EventLogSequenceNumber = event_log_sequence_number,
        };
        committed_aggregate_events = new PbCommittedAggregateEvents
        {
            EventSourceId = event_source.Value,
            AggregateRootId = aggregate_root_id.ToProtobuf(),
            AggregateRootVersion = aggregate_root_version,
            CurrentAggregateRootVersion = aggregate_root_version,
            Events = { committed_aggregate_event }
        };

        object_from_serializer = new object();
        SetupDeserializeToReturnObject(content_string, object_from_serializer);
    };

    Because of = () => try_result = converter.TryConvert(committed_aggregate_events, out converted_events, out exception);

    It should_return_false = () => try_result.ShouldBeFalse();
    It should_out_default_events = () => converted_events.ShouldEqual(default);
    It should_out_invalid_committed_event_information = () => exception.ShouldBeOfExactType<InvalidCommittedEventInformation>();
}