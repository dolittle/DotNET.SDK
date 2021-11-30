// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Customers;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Kitchen;

[EventHandler("f2d366cf-c00a-4479-acc4-851e04b6fbba")]
public class DishServer
{
    readonly IAggregateOf<Customer> _customers;

    public DishServer(IAggregates aggregates)
    {
        _customers = aggregates.Of<Customer>();
    }

    public async Task Handle(DishPrepared @event, EventContext ctx)
    {
        Console.WriteLine($"{@event.Chef} from the {ctx.EventSourceId} kitchen has prepared {@event.Dish}. Serving dish to Jakob. Bon apetite!");
        await _customers
            .Get("Jakob")
            .Perform(_ => _.EatDish(@event.Dish))
            .ConfigureAwait(false);
    }
}

