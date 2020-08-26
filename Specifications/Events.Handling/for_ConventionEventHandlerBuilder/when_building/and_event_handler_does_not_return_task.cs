// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.when_building
{
    public class and_event_handler_does_not_return_task : given.event_handlers_and_factories
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => ConventionEventHandlerBuilder<IEvent>.BuildFor(event_handler_that_does_not_return_task_factory));

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventHandlerMethodMustReturnATask>();
    }
}