// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.when_building
{
    public class and_event_handler_is_correctly_defined : given.event_handlers_and_factories
    {
        static Exception exception;
        static IEventHandler<IEvent> handler;

        Because of = () => exception = Catch.Exception(() => handler = ConventionEventHandlerBuilder<IEvent>.BuildFor(event_handler_factory));

        It should_not_throw_an_exception = () => exception.ShouldBeNull();
        It should_return_an_event_handler = () => handler.ShouldNotBeNull();
    }
}