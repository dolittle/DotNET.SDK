// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_a_committed_event
{
    public class and_event_source_is_a_guid : given.a_committed_event_and_a_converter
    {
        static Guid event_source_guid;
        static object object_from_serializer;

        static bool try_result;
        static CommittedEvent converted_event;
        static Exception exception;

        Establish context = () =>
        {
            event_source_guid = Guid.Parse("2248f2a2-81ce-426b-afc3-52e711f6985a");
            event_source = event_source_guid;

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

            object_from_serializer = new object();
            SetupDeserializeToReturnObject(content_string, object_from_serializer);
        };

        Because of = () => try_result = converter.TryConvert(committed_event, out converted_event, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It should_have_the_correct_event_source = () => converted_event.EventSource.Value.ShouldEqual(event_source_guid.ToString());
    }
}
