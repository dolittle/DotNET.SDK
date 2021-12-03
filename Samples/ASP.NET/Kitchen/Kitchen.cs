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
    readonly HashSet<string> _chefs = new();

    public Kitchen(EventSourceId eventSource)
        : base(eventSource)
    {
    }

    string Name => EventSourceId;

    public void RestockIngredient(string ingredient, int amount)
    {
        var newStock = amount;
        if (_ingredients.ContainsKey(ingredient))
        {
            newStock += _ingredients[ingredient];
        }
        
        Apply(new IngredientRestocked(ingredient, amount, newStock));
    }

    public void CheckInChef(string chef)
    {
        if (_chefs.Contains(chef))
        {
            throw new Exception($"Chef {chef} has already checked in to kitchen {Name}");
        }
        Apply(new ChefCheckedIn(chef));
    }
    public void CheckOutChef(string chef)
    {
        if (!_chefs.Contains(chef))
        {
            throw new Exception($"Chef {chef} has not checked in to kitchen {Name}");
        }
        Apply(new ChefCheckedOut(chef));
    }

    public void PrepareDish(string chef, string dish, IDictionary<string, int> requiredIngredients)
    {
        // foreach (var (ingredient, amount) in requiredIngredients)
        // {
            
        // }
        // if ( <= 0)
        // {
        //     throw new Exception($"Kitchen {Name} has run out of ingredients, sorry!");
        // }
        Apply(new DishPrepared(dish, chef));
        Console.WriteLine($"Kitchen {EventSourceId} prepared a {dish}, there are {_ingredients} ingredients left.");
    }

    void On(ChefCheckedIn @event)
        => _chefs.Add(@event.Chef);
    
    void On(ChefCheckedOut @event)
        => _chefs.Remove(@event.Chef);

    void On(IngredientUsed @event)
        => _ingredients[@event.Ingredient] = @event.Stock;
    
    void On(IngredientRestocked @event)
        => _ingredients[@event.Ingredient] = @event.Stock; 
}