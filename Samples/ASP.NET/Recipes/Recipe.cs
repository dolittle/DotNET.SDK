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
        if (!_descriptionSet)
        {
            
        }
    }

    public void UpdateIngredientNeeded(string ingredient, int amount)
    {

    }
}

public record RecipeDescripionAdded(string Description);

public record RecipeDescriptionChanged(string Description);

public record IngredientAddedToRecipe(string Ingredient, int AmountNeeded);

public record IngredientRemovedFromRecipe(string Ingredient, int AmountNeeded);

public record IngredientAmountChangedForRecipe(string Ingredient, int NewAmountNeeded, int OldAmountNeeded);
