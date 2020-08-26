// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.when_building
{
    public class and_external_event_handler_handles_private_event : given.event_handlers_and_factories
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => ConventionEventHandlerBuilder<IPublicEvent>.BuildFor(external_event_handler_with_private_event_factory));

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventHandlerMethodFirstParameterMustBeCorrectEventType>();
    }
}