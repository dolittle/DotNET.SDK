// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Customers;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Kitchen;
using Microsoft.Extensions.Logging;

[EventHandler("f2d366cf-c00a-4479-acc4-851e04b6fbba")]
public class DishServer
{
    public const string CustomerName = "Jakob";
    readonly IAggregateOf<Customer> _customers;
    readonly ILogger<DishServer> _logger;

    public DishServer(IAggregateOf<Customer> customers, ILogger<DishServer> logger)
    {
        _customers = customers;
        _logger = logger;
    }

    public async Task Handle(DishPrepared @event, EventContext ctx)
    {
        _logger.LogInformation("{Chef} from the {Kitchen} kitchen has prepared {Dish}. Serving dish to Jakob. Bon apetite!", @event.Chef, ctx.EventSourceId, @event.Dish);
        await _customers
            .Get(CustomerName)
            .Perform(_ => _.EatDish(@event.Dish))
            .ConfigureAwait(false);
    }
}
