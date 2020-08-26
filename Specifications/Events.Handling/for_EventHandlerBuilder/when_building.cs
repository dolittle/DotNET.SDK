// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Handling.for_EventHandlerBuilder
{
    public class when_building
    {
        static IEventHandler<IEvent> handler;

        Because of = () => handler = EventHandlerBuilder.Create<IEvent>().Handle((MyFirstEvent e, EventContext c) => Task.CompletedTask).Build();

        It should_return_an_event_handler = () => handler.ShouldNotBeNull();
    }
}