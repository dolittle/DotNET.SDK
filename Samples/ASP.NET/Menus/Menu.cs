// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Restaurant.Menus;

[AggregateRoot("dcdaecc0-29c9-41f4-96d1-9bddefe8b39a")]
public class Menu : AggregateRoot
{
    readonly HashSet<string> _dishes = new();

    public Menu(EventSourceId restaurant) : base(restaurant)
    {
    }

    string Restaurant => EventSourceId;

    public void AddDish(string dish)
    {
        if (_dishes.Contains(dish))
        {
            throw new ArgumentException($"Menu for restaurant {Restaurant} already contains dish {dish}", nameof(dish));
        }
        Apply(new DishAddedToMenu(dish));
    }

    public void RemoveDish(string dish)
    {
        if (!_dishes.Contains(dish))
        {
            throw new ArgumentException($"Menu for restaurant {Restaurant} does not have dish {dish}", nameof(dish));
        }
        Apply(new DishRemovedFromMenu(dish));
    }

    void On(DishAddedToMenu @event)
        => _dishes.Add(@event.Dish);

    void On(DishRemovedFromMenu @event)
        => _dishes.Remove(@event.Dish);
}