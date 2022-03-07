// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling;

public class an_event_type_that_is_not_handled : given.all_dependencies
{
    static EventType event_type;
    static Exception exception;

    Establish context = () => event_type = new EventType("4b6ace2e-b95f-485e-a68f-75b944218391");

    Because of = () => exception = Catch.Exception(() => event_handler.Handle(new object(), event_type, event_context, service_provider.Object, CancellationToken.None).GetAwaiter().GetResult());

    It should_throw_exception_because_event_handler_does_not_handle_event_type = () => exception.ShouldBeOfExactType<MissingEventHandlerForEventType>();
}