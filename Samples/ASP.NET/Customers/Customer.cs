// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

namespace Customers;
[AggregateRoot("dcdaecc0-29c9-41f4-96d1-9bddefe8b39a")]
public class Customer : AggregateRoot
{
    readonly ILogger<Customer> _logger;

    public Customer(EventSourceId eventSource, ILogger<Customer> logger)
        : base(eventSource)
    {
        _logger = logger;
    }

    string Name => EventSourceId;

    public void EatDish(string dish)
    {
        Apply(new DishEaten(dish, Name));
        _logger.LogInformation("Customer {Name} is eating {Dish}.", Name, dish);
    }
}