// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_EventHandlerBuilder
{
    public class when_invoking_a_factory_handler
    {
        static MyFirstEvent first_event;
        static MyFirstEvent second_event;
        static IList<EventHandler> created_event_handlers;
        static IEventHandler<IEvent> handler;

        Establish context = () =>
        {
            first_event = new MyFirstEvent();
            second_event = new MyFirstEvent();

            created_event_handlers = new List<EventHandler>();

            handler = EventHandlerBuilder.Create<IEvent>()
                .With(() =>
                    {
                        var handler = new EventHandler();
                        created_event_handlers.Add(handler);
                        return handler;
                    })
                .Handle((EventHandler h, MyFirstEvent e, EventContext c) => h.Handle(e, c))
                .Build();
        };

        Because of = () =>
        {
            handler.Handle(first_event, null);
            handler.Handle(second_event, null);
        };

        It should_create_two_event_handlers = () => created_event_handlers.Count.ShouldEqual(2);
        It should_use_the_first_instance_for_the_first_event = () => created_event_handlers[0].HandledEvents.ShouldContainOnly(first_event);
        It should_use_the_second_instance_for_the_second_event = () => created_event_handlers[1].HandledEvents.ShouldContainOnly(second_event);
    }
}