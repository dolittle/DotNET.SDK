// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.given
{
    public class a_committed_event_and_a_converter : a_converter_and_a_protobuf_execution_context
    {
        protected static string content_string;
        protected static bool is_public;
        protected static EventType event_type;
        protected static EventSourceId event_source;
        protected static DateTimeOffset occured;
        protected static EventLogSequenceNumber event_log_sequence_number;
        protected static PbCommittedEvent committed_event;

        Establish context = () =>
        {
            content_string = "medowfecavremru";
            is_public = false;
            event_type = new EventType("926a967f-3833-47ef-82af-95612b826015", 14);
            event_source = "Jimmy Frank";
            occured = new DateTimeOffset(2018, 5, 12, 22, 17, 19, TimeSpan.Zero);
            event_log_sequence_number = 3448072883;

            committed_event = new PbCommittedEvent
            {
                External = false,
                Content = content_string,
                Public = is_public,
                Type = event_type.ToProtobuf(),
                ExecutionContext = execution_context,
                EventSourceId = event_source.Value,
                Occurred = Timestamp.FromDateTimeOffset(occured),
                EventLogSequenceNumber = event_log_sequence_number,
            };
        };
    }
}
