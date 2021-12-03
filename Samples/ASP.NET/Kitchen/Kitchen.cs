// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

// namespace Kitchen;

[AggregateRoot("01ad9a9f-711f-47a8-8549-43320f782a1e")]
public class Kitchen : AggregateRoot
{
    readonly Dictionary<string, int> _ingredients = new();

    public Kitchen(EventSourceId eventSource)
        : base(eventSource)
    {
    }

    string Name => EventSourceId;

    public void RestockIngredient(string ingredient, int amount)
    {
        var oldAmount = 0;
        if (_ingredients.ContainsKey(ingredient))
        {
            oldAmount = _ingredients[ingredient];
        }
        _ingredients[ingredient] = oldAmount + amount; 
        Apply(new );
    }

    public void PrepareDish(string chef, string dish)
    {
        if (_ingredients <= 0) throw new Exception($"Kitchen {Name} has run out of ingredients, sorry!");
        Apply(new DishPrepared(dish, chef));
        Console.WriteLine($"Kitchen {EventSourceId} prepared a {dish}, there are {_ingredients} ingredients left.");
    }

    void On(DishPrepared @event)
        => _ingredients--;
}