// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new ArgumentException($"Chef {chef} has already checked in to kitchen {Name}", nameof(chef));
        }
        Apply(new ChefCheckedIn(chef));
    }
    public void CheckOutChef(string chef)
    {
        if (!_chefs.Contains(chef))
        {
            throw new ArgumentException($"Chef {chef} has not checked in to kitchen {Name}", nameof(chef));
        }
        Apply(new ChefCheckedOut(chef));
    }

    public void PrepareDish(string chef, string dish, IDictionary<string, int> requiredIngredients)
    {
        if (HasInsufficientIngredients(requiredIngredients, out var insufficientIngredients))
        {
            throw new ArgumentException(
                $"Kitchen {EventSourceId} has insufficient ingredients\n{string.Join("\n", insufficientIngredients.Select(_ => $"\t{_.Key}: {_.Value}") )}",
                nameof(requiredIngredients));
        }
        Apply(new DishPrepared(dish, chef));
        foreach (var (ingredient, amount) in requiredIngredients)
        {
            Apply(new IngredientUsed(ingredient, amount, _ingredients[ingredient] - amount));
        }
    }

    bool HasInsufficientIngredients(IDictionary<string, int> requiredIngredients, out IDictionary<string, int> insufficientIngredients)
    {
        insufficientIngredients = new Dictionary<string, int>();
        foreach (var (ingredient, amount) in requiredIngredients)
        {
            _ingredients.TryGetValue(ingredient, out var stock);
            if (stock < amount)
            {
                insufficientIngredients[ingredient] = amount - stock;
            }
        }
        return insufficientIngredients.Any();
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