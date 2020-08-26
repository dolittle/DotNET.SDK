// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_EventHandlerBuilder
{
    public class when_invoking_a_built_handler
    {
        static IEventHandler<IEvent> handler;
        static IList<IEvent> handled_events;
        static MyThirdEvent first_event;
        static MyFirstEvent second_event;
        static MyFirstEvent third_event;
        static MySecondEvent fourth_event;

        Establish context = () =>
        {
            first_event = new MyThirdEvent();
            second_event = new MyFirstEvent();
            third_event = new MyFirstEvent();
            fourth_event = new MySecondEvent();

            handled_events = new List<IEvent>();

            var builder = EventHandlerBuilder.Create<IEvent>();
            builder.Handle((MyFirstEvent e, EventContext c) =>
            {
                handled_events.Add(e);
                return Task.CompletedTask;
            });
            builder.Handle((MySecondEvent e, EventContext c) =>
            {
                handled_events.Add(e);
                return Task.CompletedTask;
            });
            builder.Handle((MyThirdEvent e, EventContext c) =>
            {
                handled_events.Add(e);
                return Task.CompletedTask;
            });
            handler = builder.Build();
        };

        Because of = () =>
        {
            handler.Handle(first_event, null);
            handler.Handle(second_event, null);
            handler.Handle(third_event, null);
            handler.Handle(fourth_event, null);
        };

        It should_call_the_correct_handlers_in_order = () => handled_events.ShouldContainOnly(first_event, second_event, third_event, fourth_event);
    }
}