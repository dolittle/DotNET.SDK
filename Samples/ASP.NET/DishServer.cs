// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Customers;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Kitchen;

[EventHandler("f2d366cf-c00a-4479-acc4-851e04b6fbba")]
public class DishServer
{
    readonly IAggregates _aggregates;

    public DishServer(IAggregates aggregates)
    {
        _aggregates = aggregates;
    }

    public async Task Handle(DishPrepared @event, EventContext ctx)
    {
        Console.WriteLine($"{@event.Chef} from the {ctx.EventSourceId} kitchen has prepared {@event.Dish}. Serving dish to Jakob. Bon apetite!");
        await _aggregates
            .Get<Customer>("Jakob")
            .Perform(_ => _.EatDish(@event.Dish))
            .ConfigureAwait(false);
    }
}

