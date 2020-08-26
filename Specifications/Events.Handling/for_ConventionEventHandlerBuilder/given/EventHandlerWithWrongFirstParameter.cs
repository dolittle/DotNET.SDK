// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.given
{
    public class EventHandlerWithWrongFirstParameter : ICanHandleEvents
    {
        public Task Handle(string @event, EventContext context)
            => Task.CompletedTask;
    }
}