// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// namespace Recipes;

using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

public class Recipe : AggregateRoot
{
    bool _descriptionIsSet;
    string _description;
    readonly Dictionary<string, int> _ingredientsNeeded = new();

    public Recipe(EventSourceId eventSource) : base(eventSource)
    {}

    public void SetDescription(string description)
    {
        if (!_descriptionIsSet)
        {
            Apply(new RecipeDescripionAdded(description));
        }
        else
        {
            Apply(new RecipeDescriptionChanged(description));
        }
    }

    public void UpdateIngredientNeeded(string ingredient, int amount)
    {
        var newAmount = amount;
        if (!_ingredientsNeeded.TryGetValue(ingredient, out var oldAmount))
        {
            Apply(new IngredientAddedToRecipe(ingredient, amount));
        }
        else
        {
            Apply(amount == 0 ? new IngredientRemovedFromRecipe(ingredient) : new IngredientAmountChangedForRecipe(ingredient, amount, oldAmount));
        }
    }

    void On(RecipeDescripionAdded @event)
        => SetInternalDescription(@event.Description);

    void On(RecipeDescriptionChanged @event)
        => SetInternalDescription(@event.Description);

    void On(IngredientAddedToRecipe @event)
        => _ingredientsNeeded[@event.Ingredient] = @event.AmountNeeded;
    
    void On(IngredientAmountChangedForRecipe @event)
        => _ingredientsNeeded[@event.Ingredient] = @event.NewAmountNeeded;

    void On(IngredientRemovedFromRecipe @event)
        => _ingredientsNeeded.Remove(@event.Ingredient); 

    void SetInternalDescription(string description)
    {
        _descriptionIsSet = true;
        _description = description;
    }
}