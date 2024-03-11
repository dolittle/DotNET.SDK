// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using PbCommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents;

#pragma warning disable CS0612

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToSDKConverter.when_converting_committed_aggregate_events;

public class and_event_source_is_a_guid : given.committed_aggregate_events_and_a_converter
{
    static Guid event_source_guid;
    static object object_from_serializer;

    static bool try_result;
    static CommittedAggregateEvents converted_events;
    static Exception exception;

    Establish context = () =>
    {
        event_source_guid = Guid.Parse("57d9d2b6-0a37-4793-a7d6-f4caad4f7694");
        event_source = event_source_guid;
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

    It should_return_true = () => try_result.ShouldBeTrue();
    It should_have_no_exception = () => exception.ShouldBeNull();
    It should_have_the_correct_event_source = () => converted_events.EventSource.Value.ShouldEqual(event_source_guid.ToString());
}