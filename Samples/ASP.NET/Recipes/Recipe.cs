// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Recipes;
 
[AggregateRoot("b0dcbfef-9e49-4c44-a286-45ff968b5086")]
public class Recipe : AggregateRoot
{
    bool _descriptionIsSet;
    readonly Dictionary<string, int> _ingredientsNeeded = new();

    public Recipe(EventSourceId recipe) : base(recipe)
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
        if (!_ingredientsNeeded.TryGetValue(ingredient, out var oldAmount))
        {
            Apply(new IngredientAddedToRecipe(ingredient, amount));
        }
        else if (amount == 0)
        {
            Apply(new IngredientRemovedFromRecipe(ingredient));
        }
        else
        {
            Apply(new IngredientAmountChangedForRecipe(ingredient, amount, oldAmount));
        }
    }

    void On(RecipeDescripionAdded @event)
        => _descriptionIsSet = true;

    void On(RecipeDescriptionChanged @event)
        => _descriptionIsSet = true;

    void On(IngredientAddedToRecipe @event)
        => _ingredientsNeeded[@event.Ingredient] = @event.AmountNeeded;
    
    void On(IngredientAmountChangedForRecipe @event)
        => _ingredientsNeeded[@event.Ingredient] = @event.NewAmountNeeded;

    void On(IngredientRemovedFromRecipe @event)
        => _ingredientsNeeded.Remove(@event.Ingredient); 

}