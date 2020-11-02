// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace ASP.NET 
{
    [EventType("c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb")]
    public class DishEaten
    {
        public DishEaten (string dish, string customer)
        {
            Dish = dish;
            Customer = customer;
        }

        public string Dish { get; }
        public string Customer { get; }
    }
}
