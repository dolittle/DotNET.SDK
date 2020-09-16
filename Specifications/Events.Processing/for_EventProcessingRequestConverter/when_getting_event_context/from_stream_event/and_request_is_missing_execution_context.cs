// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.when_getting_event_context.from_stream_event
{
    public class and_request_is_missing_execution_context : given.all_dependencies
    {
        static StreamEvent stream_event;
        static Exception exception;

        Establish context = () =>
        {
            committed_event.ExecutionContext = default;
            stream_event = new StreamEvent { Event = committed_event };
        };

        Because of = () => exception = Catch.Exception(() => converter.GetEventContext(stream_event));

        It should_fail_because_missing_event_information = () => exception.ShouldBeOfExactType<MissingEventInformation>();
    }
}