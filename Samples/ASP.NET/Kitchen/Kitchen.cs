// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

// namespace Kitchen;

[AggregateRoot("01ad9a9f-711f-47a8-8549-43320f782a1e")]
public class Kitchen : AggregateRoot
{
    readonly Dictionary<string, int> _ingredients = new();
    readonly Dictionary<string, DateTimeOffset> _chefs = new();

    public Kitchen(EventSourceId restaurant) : base(restaurant)
    {
    }

    string Restaurant => EventSourceId;

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
        if (_chefs.ContainsKey(chef))
        {
            throw new ArgumentException($"Chef {chef} has already checked in to kitchen {Restaurant}", nameof(chef));
        }
        Apply(new ChefCheckedIn(chef, DateTimeOffset.Now));
    }
    
    public void CheckOutChef(string chef)
    {
        if (!_chefs.ContainsKey(chef))
        {
            throw new ArgumentException($"Chef {chef} has not checked in to kitchen {Restaurant}", nameof(chef));
        }

        var checkedInTime = _chefs[chef];
        var timeAtWork = DateTimeOffset.Now - checkedInTime;
        Apply(new ChefCheckedOut(chef, (int)Math.Round(timeAtWork.TotalHours + 0.5)));
    }

    public void PrepareDish(string dish, IDictionary<string, int> requiredIngredients)
    {
        if (HasInsufficientIngredients(requiredIngredients, out var insufficientIngredients))
        {
            throw new ArgumentException(
                $"Kitchen {EventSourceId} has insufficient ingredients\n{string.Join("\n", insufficientIngredients.Select(_ => $"\t{_.Key}: {_.Value}") )}",
                nameof(requiredIngredients));
        }

        if (_chefs.Count < 1)
        {
            throw new EntryPointNotFoundException(
                $"There are no chefs available to prepare {dish} at the {Restaurant} restaurant.");
        }
        
        Apply(new DishPrepared(dish, PickChefToPrepareNextDish()));
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

    string PickChefToPrepareNextDish()
    {
        var availableChefs = _chefs.Keys.ToArray(); 
        var index = RandomNumberGenerator.GetInt32(0, availableChefs.Length);
        return availableChefs[index];
    }

    void On(ChefCheckedIn @event)
        => _chefs[@event.Chef] = @event.CheckInTime;
    
    void On(ChefCheckedOut @event)
        => _chefs.Remove(@event.Chef);

    void On(IngredientUsed @event)
        => _ingredients[@event.Ingredient] = @event.Stock;
    
    void On(IngredientRestocked @event)
        => _ingredients[@event.Ingredient] = @event.Stock; 
}