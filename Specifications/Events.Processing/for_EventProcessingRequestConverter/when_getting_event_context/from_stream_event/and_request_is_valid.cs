// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.when_getting_event_context.from_stream_event
{
    public class and_request_is_valid : given.all_dependencies
    {
        static StreamEvent stream_event;
        static EventContext result;

        Establish context = () => stream_event = new StreamEvent { Event = committed_event };
        Because of = () => result = converter.GetEventContext(stream_event);

        It should_have_the_correct_event_log_sequence_number = () => result.SequenceNumber.Value.ShouldEqual(committed_event.EventLogSequenceNumber);
        It should_have_the_correct_event_source_id = () => result.EventSourceId.ShouldEqual(committed_event.EventSourceId.To<EventSourceId>());
        It should_have_the_correct_execution_context = () => result.ExecutionContext.ShouldEqual(committed_event.ExecutionContext.ToExecutionContext());
        It should_have_the_correct_ocurred_date = () => result.Occurred.ShouldEqual(committed_event.Occurred.ToDateTimeOffset());
    }
}