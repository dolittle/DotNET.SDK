// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Customers;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Microsoft.Extensions.Logging;

[EventHandler("86dd35ee-cd28-48d9-a0cd-cb2aa11851cb")]
public class DishEater
{
    readonly ILogger<DishEater> _logger;

    public DishEater(ILogger<DishEater> logger)
    {
        _logger = logger;
    }

    public void Handle(DishEaten @event, EventContext ctx)
    {
        _logger.LogInformation($"{ctx.EventSourceId} has eaten {@event.Dish}. Yummm!");
    }
}
