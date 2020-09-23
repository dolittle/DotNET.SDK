// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor.when_handling.a_request_to_handle_event
{
    public class and_handler_fails : given.all_dependencies
    {
        static EventContext event_context;
        static EventHandlerResponse response;

        Establish context = () =>
        {
            event_context = new EventContext(
                committed_event.EventLogSequenceNumber,
                committed_event.EventSourceId.To<EventSourceId>(),
                committed_event.Occurred.ToDateTimeOffset(),
                execution_context,
                execution_context);
            event_types.Setup(_ => _.HasTypeFor(event_type_to_handle)).Returns(true);
            event_types.Setup(_ => _.GetTypeFor(event_type_to_handle)).Returns(typeof(given.some_event));
            event_handler
                .Setup(_ => _.Handle(Moq.It.IsAny<object>(), Moq.It.IsAny<EventType>(), Moq.It.IsAny<EventContext>(), Moq.It.IsAny<CancellationToken>()))
                .Throws(new System.Exception());
        };

        Because of = () => response = event_handler_processor.Handle(request, execution_context, CancellationToken.None).GetAwaiter().GetResult();

        It should_get_a_response = () => response.ShouldNotBeNull();
        It should_have_a_failure = () => response.Failure.ShouldNotBeNull();
        It should_try_to_retry_processing = () => response.Failure.Retry.ShouldBeTrue();
        It should_have_called_the_handler = () => event_handler.Verify(_ => _.Handle(event_to_handle, event_type_to_handle, event_context, CancellationToken.None), Moq.Times.Once);
    }
}