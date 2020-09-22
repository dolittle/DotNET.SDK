// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor.when_handling.a_request_to_handle_event.and_stream_event.has_committed_event
{
    public class with_event_that_cannot_be_deserialized : given.all_dependencies
    {
        static EventHandlerResponse response;

        Establish context = () =>
        {
            var event_type = request.Event.Event.Type.To<EventType>();
            event_types.Setup(_ => _.HasTypeFor(event_type)).Returns(true);
            event_types.Setup(_ => _.GetTypeFor(event_type)).Returns(typeof(given.some_event));
            request.Event.Event.Content = "{\"something\": \"hello\"";
        };

        Because of = () => response = event_handler_processor.Handle(request, execution_context, CancellationToken.None).GetAwaiter().GetResult();

        It should_get_a_response = () => response.ShouldNotBeNull();
        It should_have_a_failure = () => response.Failure.ShouldNotBeNull();
        It should_try_to_retry_processing = () => response.Failure.Retry.ShouldBeTrue();
    }
}