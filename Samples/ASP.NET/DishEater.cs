// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Customers;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Microsoft.Extensions.Logging;

[EventHandler("86dd35ee-cd28-48d9-a0cd-cb2aa11851cc", partitioned:false)]
public class DishEater2
{
    readonly ILogger<DishEater2> _logger;

    public DishEater2(ILogger<DishEater2> logger)
    {
        _logger = logger;
    }

    public void Handle(DishEaten @event, EventContext ctx)
    {
        _logger.LogInformation("{CtxEventSourceId} has eaten unpartitioned {EventDish}. Yummm!", ctx.EventSourceId, @event.Dish);
    }
}
