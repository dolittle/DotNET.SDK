// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.when_getting_event_context.from_committed_event
{
    public class and_request_is_missing_event_source_id : given.all_dependencies
    {
        static Exception exception;

        Establish context = () => committed_event.EventSourceId = default;

        Because of = () => exception = Catch.Exception(() => converter.GetEventContext(committed_event));

        It should_fail_because_missing_event_information = () => exception.ShouldBeOfExactType<MissingEventInformation>();
    }
}