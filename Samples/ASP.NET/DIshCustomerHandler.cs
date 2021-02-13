// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Handling;

namespace ASP.NET
{
    [EventHandler("86dd35ee-cd28-48d9-a0cd-cb2aa11851cb")]
    public class DishCustomerHandler
    {
        public void Handle(DishEaten @event, EventContext eventContext)
        {
            Console.WriteLine($"{@event.Customer} has eaten {@event.Dish}. Yummm!");
        }
    }
}
