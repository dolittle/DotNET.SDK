// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Projections;

namespace Recipes;

[Projection("0f72d1d2-1f00-4d58-98e8-9d16d75a7d2e")]
public class RecipeIngredients
{
    public List<Ingredient> Ingredients { get; } = new();

    [KeyFromEventSource]
    public void On(IngredientAddedToRecipe @event, ProjectionContext ctx)
        => Ingredients.Add(new Ingredient{ Name = @event.Ingredient, AmountNeeded = @event.AmountNeeded });

    [KeyFromEventSource]
    public void On(IngredientRemovedFromRecipe @event, ProjectionContext ctx)
        => Ingredients.RemoveAll(_ => _.Name == @event.Ingredient);

    [KeyFromEventSource]
    public void On(IngredientAmountChangedForRecipe @event, ProjectionContext ctx)
    {
        var ingredient = Ingredients.Find(_ => _.Name == @event.Ingredient);
        if (ingredient != null)
        {
            ingredient.AmountNeeded = @event.NewAmountNeeded;
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public int AmountNeeded { get; set; }
    };
}