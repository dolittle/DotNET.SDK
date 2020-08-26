// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.Events.Handling
{
    public class EventHandler : ICanHandleEvents
    {
        readonly IList<IEvent> _handledEvents = new List<IEvent>();

        public IEnumerable<IEvent> HandledEvents => _handledEvents;

        public Task Handle(MyFirstEvent @event, EventContext context)
        {
            _handledEvents.Add(@event);
            return Task.CompletedTask;
        }

        public Task Handle(MySecondEvent @event, EventContext context)
        {
            _handledEvents.Add(@event);
            return Task.CompletedTask;
        }

        public Task Handle(MyThirdEvent @event, EventContext context)
        {
            _handledEvents.Add(@event);
            return Task.CompletedTask;
        }
    }
}