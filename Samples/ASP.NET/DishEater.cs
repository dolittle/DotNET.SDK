// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Customers;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventHandler("86dd35ee-cd28-48d9-a0cd-cb2aa11851cb")]
public class DishEater
{
    public void Handle(DishEaten @event, EventContext ctx)
    {
        Console.WriteLine($"{ctx.EventSourceId} has eaten {@event.Dish}. Yummm!");
    }
}
