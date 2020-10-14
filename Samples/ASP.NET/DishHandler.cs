// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Handling;

namespace ASP.NET
{
    [EventHandler("f2d366cf-c00a-4479-acc4-851e04b6fbba")]
    public class DishHandler
    {
        readonly IEventStore _eventStore;

        public DishHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DishPrepared @event, EventContext eventContext)
        {
            Console.WriteLine($"{@event.Chef} has prepared {@event.Dish}. Bon apetite!");
            await _eventStore.CommitEvent(new DishEaten(@event.Dish, "Jakob"), eventContext.EventSourceId);
        }
    }
}
