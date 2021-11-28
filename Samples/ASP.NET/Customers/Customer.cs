// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Customers;
[AggregateRoot("dcdaecc0-29c9-41f4-96d1-9bddefe8b39a")]
public class Customer : AggregateRoot
{
    public Customer(EventSourceId eventSource)
        : base(eventSource)
    {
    }

    string Name => EventSourceId;

    public void EatDish(string dish)
    {
        Apply(new DishEaten(dish, EventSourceId));
        Console.WriteLine($"Customer {Name} is eating {dish}.");
    }
}