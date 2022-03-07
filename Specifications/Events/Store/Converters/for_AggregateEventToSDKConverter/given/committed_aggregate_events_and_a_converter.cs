// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbCommittedAggregateEvent = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent;
using PbCommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToSDKConverter.given;

public class committed_aggregate_events_and_a_converter : a_converter_and_a_protobuf_execution_context
{
    protected static string content_string;
    protected static bool is_public;
    protected static EventType event_type;
    protected static EventSourceId event_source;
    protected static AggregateRootId aggregate_root_id;
    protected static AggregateRootVersion aggregate_root_version;
    protected static DateTimeOffset occured;
    protected static EventLogSequenceNumber event_log_sequence_number;
    protected static PbCommittedAggregateEvent committed_aggregate_event;
    protected static PbCommittedAggregateEvents committed_aggregate_events;

    Establish context = () =>
    {
        content_string = "ojmikemitethulotlidruewim";
        is_public = false;
        event_type = new EventType("18af5922-9cb2-4a26-bab8-513eb5e00d60", 1589312422);
        event_source = "Rena Pearson";
        aggregate_root_id = "717915c1-bb88-4bec-b1c1-61451c5a6608";

        aggregate_root_version = 186206759u;
        occured = new DateTimeOffset(2018, 5, 12, 22, 17, 19, TimeSpan.Zero);
        event_log_sequence_number = 1581420095;

        committed_aggregate_event = new PbCommittedAggregateEvent
        {
            Content = content_string,
            Public = is_public,
            EventType = event_type.ToProtobuf(),
            ExecutionContext = execution_context,
            Occurred = Timestamp.FromDateTimeOffset(occured),
            EventLogSequenceNumber = event_log_sequence_number,
        };
        committed_aggregate_events = new PbCommittedAggregateEvents
        {
            EventSourceId = event_source.Value,
            AggregateRootId = aggregate_root_id.ToProtobuf(),
            AggregateRootVersion = aggregate_root_version,
            Events = { committed_aggregate_event }
        };
    };
}