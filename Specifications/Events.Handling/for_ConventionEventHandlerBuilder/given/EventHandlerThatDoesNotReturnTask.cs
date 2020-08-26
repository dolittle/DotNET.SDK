// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.given
{
    public class EventHandlerThatDoesNotReturnTask : ICanHandleEvents
    {
        public bool Handle(MyFirstEvent @event, EventContext context)
            => true;
    }
}