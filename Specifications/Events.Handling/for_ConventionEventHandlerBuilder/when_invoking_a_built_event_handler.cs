// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.when_building
{
    public class when_invoking_a_built_event_handler : given.event_handlers_and_factories
    {
        static IEventHandler<IEvent> handler;
        static MySecondEvent first_event;
        static MyThirdEvent second_event;
        static MyFirstEvent third_event;
        static MyThirdEvent fourth_event;

        Establish context = () =>
        {
            first_event = new MySecondEvent();
            second_event = new MyThirdEvent();
            third_event = new MyFirstEvent();
            fourth_event = new MyThirdEvent();
            handler = ConventionEventHandlerBuilder<IEvent>.BuildFor(event_handler_factory);
        };

        Because of = () =>
        {
            handler.Handle(first_event, null);
            handler.Handle(second_event, null);
            handler.Handle(third_event, null);
            handler.Handle(fourth_event, null);
        };

        It should_call_the_correct_methods = () => event_handler.HandledEvents.ShouldContainOnly(first_event, second_event, third_event, fourth_event);
    }
}