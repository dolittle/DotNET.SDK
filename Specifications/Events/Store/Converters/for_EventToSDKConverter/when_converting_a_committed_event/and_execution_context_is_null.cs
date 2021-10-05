// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_a_committed_event
{
    public class and_execution_context_is_null : given.a_committed_event_and_a_converter
    {
        static bool try_result;
        static CommittedEvent converted_event;
        static Exception exception;

        Establish context = () =>
        {
            committed_event = new PbCommittedEvent
            {
                External = false,
                Content = content_string,
                Public = is_public,
                Type = event_type.ToProtobuf(),
                ExecutionContext = null,
                EventSourceId = event_source.Value,
                Occurred = Timestamp.FromDateTimeOffset(occured),
                EventLogSequenceNumber = event_log_sequence_number,
            };
        };

        Because of = () => try_result = converter.TryConvert(committed_event, out converted_event, out exception);

        It should_return_false = () => try_result.ShouldBeFalse();
        It should_out_default_event = () => converted_event.ShouldEqual(default);
        It should_out_invalid_committed_event_information = () => exception.ShouldBeOfExactType<InvalidCommittedEventInformation>();
    }
}
