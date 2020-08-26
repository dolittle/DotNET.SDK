// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_EventHandlerBuilder
{
    public class when_invoking_a_built_handler_with_an_event_it_does_not_handle
    {
        static Exception exception;
        static IEventHandler<IEvent> handler;

        Establish context = () => handler = EventHandlerBuilder.Create<IEvent>().Handle((MyFirstEvent e, EventContext c) => Task.CompletedTask).Build();

        Because of = () => exception = Catch.Exception(() => handler.Handle(new MySecondEvent(), null));

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventHandlerDoesNotHandleEvent>();
    }
}