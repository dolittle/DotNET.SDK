// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[AggregateRoot("dcdaecc0-29c9-41f4-96d1-9bddefe8b39a")]
public class Menu : AggregateRoot
{
    readonly HashSet<string> _dishes = new();

    public Menu(EventSourceId eventSource)
        : base(eventSource)
    {
    }

    string Restaurant => EventSourceId;

    public void AddDish(string dish, string description)
    {
        if (_dishes.Contains(dish))
        {
            throw new Exception($"Menu for restaurant {Restaurant} already contains dish {dish}");
        }
        Apply(new DishAddedToMenu(dish)); // Add description here?
    }
    public void RemoveDish(string dish, string description)
    {
        if (!_dishes.Contains(dish))
        {
            throw new Exception($"Menu for restaurant {Restaurant} does not have dish {dish}");
        }
        Apply(new DishRemovedFromMenu(dish)); // Add description here?
    }
}