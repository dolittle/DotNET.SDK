// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling
{
    public class and_handler_method_fails : given.an_event_handler
    {
        static object @event;
        static EventContext event_context;
        static Exception exception;

        Establish context = () =>
        {
            @event = new object();
            event_handler_method.Setup(_ => _.TryHandle(Moq.It.IsAny<object>(), Moq.It.IsAny<EventContext>())).Returns(Task.FromResult<Try>(new Exception("Something went wrong")));
            event_context = new EventContext(
                3,
                "7d387632-68d8-4187-aa8a-b7d191a1b130",
                DateTimeOffset.UtcNow,
                new Execution.ExecutionContext(
                    "98bcb77f-4259-4038-a1ec-a9a7b0b65d93",
                    "d74a685d-4bc7-4dd3-a168-20e4da0bf344",
                    Microservices.Version.NotSet,
                    "some version",
                    "281beded-b881-4f8c-a78e-260a15497adf",
                    Security.Claims.Empty,
                    CultureInfo.InvariantCulture));
        };

        Because of = () => exception = Catch.Exception(() => event_handler.Handle(@event, handled_event_type, event_context).GetAwaiter().GetResult());

        It should_invoke_handler_with_correct_arguments = () => event_handler_method.Verify(_ => _.TryHandle(@event, event_context), Moq.Times.Once);
        It should_throw_exception_because_event_handler_method_failed = () => exception.ShouldBeOfExactType<EventHandlerMethodFailed>();
    }
}
