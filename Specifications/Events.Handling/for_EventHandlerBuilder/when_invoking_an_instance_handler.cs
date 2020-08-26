// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_EventHandlerBuilder
{
    public class when_invoking_an_instance_handler
    {
        static MyFirstEvent first_event;
        static EventHandler event_handler;
        static IEventHandler<IEvent> handler;

        Establish context = () =>
        {
            first_event = new MyFirstEvent();
            event_handler = new EventHandler();
            handler = EventHandlerBuilder.Create<IEvent>().With(event_handler).Handle((EventHandler h, MyFirstEvent e, EventContext c) => h.Handle(e, c)).Build();
        };

        Because of = () =>
        {
            handler.Handle(first_event, null);
            handler.Handle(first_event, null);
        };

        It should_call_the_method_on_the_correct_instance = () => event_handler.HandledEvents.ShouldContainOnly(first_event, first_event);
    }
}