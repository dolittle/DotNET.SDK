// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Projections.Store;
using Microsoft.AspNetCore.Mvc;

namespace Recipes;

[Route("/api/recipes")]
public class Recipes : ControllerBase
{
    readonly IAggregateOf<Recipe> _recipes;
    readonly IProjectionStore _projections;

    public Recipes(IAggregateOf<Recipe> recipes, IProjectionStore projections)
    {
        _recipes = recipes;
        _projections = projections;
    }

    [HttpPatch("{name}/description")]
    public async Task<IActionResult> SetDescription(string name, string description)
    {
        await _recipes
            .Get(name)
            .Perform(_ => _.SetDescription(description))
            .ConfigureAwait(false);

        return Ok();
    }

    [HttpPut("{name}/{ingredient}")]
    public async Task<IActionResult> UpdateIngredientNeeded(string name, string ingredient, int amount)
    {
        await _recipes
            .Get(name)
            .Perform(_ => _.UpdateIngredientNeeded(ingredient, amount))
            .ConfigureAwait(false);

        return Ok();
    }

    [HttpGet("{name}/ingredients")]
    public async Task<ActionResult<IEnumerable<RecipeIngredients.Ingredient>>> Ingredients(string name)
    {
        var ingredients = await _projections
            .Get<RecipeIngredients>(name)
            .ConfigureAwait(false);

        if (ingredients.WasCreatedFromInitialState)
        {
            return NotFound($"Recipe named {name} has not been defined");
        }
        
        return Ok(ingredients.State.Ingredients);
    }
}