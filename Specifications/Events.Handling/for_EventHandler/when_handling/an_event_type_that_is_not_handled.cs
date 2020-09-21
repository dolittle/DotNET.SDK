// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling
{
    public class an_event_type_that_is_not_handled : given.an_event_handler
    {
        static EventType event_type;
        static EventContext event_context;
        static Exception exception;

        Establish context = () =>
        {
            event_type = new EventType("4b6ace2e-b95f-485e-a68f-75b944218391");
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

        Because of = () => exception = Catch.Exception(() => event_handler.Handle(new object(), event_type, event_context).GetAwaiter().GetResult());

        It should_throw_exception_because_event_handler_does_not_handle_event_type = () => exception.ShouldBeOfExactType<MissingEventHandlerForEventType>();
    }
}
