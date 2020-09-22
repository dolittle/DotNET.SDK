// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor.when_handling.a_request_to_handle_event.and_stream_event.has_committed_event
{
    public class with_missing_execution_context : given.all_dependencies
    {
        static EventHandlerResponse response;

        Establish context = () => request.Event.Event.ExecutionContext = null;

        Because of = () => response = event_handler_processor.Handle(request, execution_context, CancellationToken.None).GetAwaiter().GetResult();

        It should_get_a_response = () => response.ShouldNotBeNull();
        It should_have_a_failure = () => response.Failure.ShouldNotBeNull();
        It should_try_to_retry_processing = () => response.Failure.Retry.ShouldBeTrue();
    }
}