// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor.when_handling.a_request_to_handle_event
{
    public class and_handler_does_not_fail : given.all_dependencies
    {
        static EventHandlerResponse response;

        Establish context = () => event_handler.Setup(_ => _.Handle(Moq.It.IsAny<object>(), Moq.It.IsAny<EventType>(), Moq.It.IsAny<EventContext>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        Because of = () => response = event_handler_processor.Handle(request, execution_context, CancellationToken.None).GetAwaiter().GetResult();

        It should_get_a_response = () => response.ShouldNotBeNull();
        It should_not_have_a_failure = () => response.Failure.ShouldBeNull();
    }
}