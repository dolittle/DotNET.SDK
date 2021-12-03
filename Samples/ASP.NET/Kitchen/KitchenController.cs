// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Resources;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Recipes;
using Restaurant.Menus;

// namespace Kitchen;

[Route("/api/kitchen")]
public class KitchenController : ControllerBase
{
    readonly IAggregateOf<Kitchen> _kitchen;
    readonly IResources _resources;
    readonly IProjectionStore _projections;

    public KitchenController(IAggregateOf<Kitchen> kitchen, IResources resources, IProjectionStore projections)
    {
        _kitchen = kitchen;
        _resources = resources;
        _projections = projections;
    }

    [HttpPost("{restaurant}/prepare")]
    public async Task<IActionResult> Prepare(string restaurant, string dish)
    {
        var database = await _resources.MongoDB.GetDatabase();
        var menu = await database
            .GetCollection<RestaurantMenu.Menu>("menus")
            .Find(Builders<RestaurantMenu.Menu>.Filter.Eq(_ => _.Restaurant, restaurant))
            .SingleOrDefaultAsync().ConfigureAwait(false);

        if (menu == null)
        {
            return NotFound($"No restaurant named {restaurant} exists.");
        }

        if (menu.Dishes.All(_ => _.Name != dish))
        {
            return NotFound($"The dish named {dish} is not on the menu :(");
        }

        var ingredients = await _projections
            .Get<RecipeIngredients>(dish)
            .ConfigureAwait(false);

        var requiredIngredients = ingredients.State.Ingredients.ToDictionary(_ => _.Name, _ => _.AmountNeeded);

        await _kitchen
            .Get(restaurant)
            .Perform(_ => _.PrepareDish(dish, requiredIngredients))
            .ConfigureAwait(false);
        
        return Ok();
    }

    [HttpPatch("{restaurant}/restock/{ingredient}")]
    public async Task<IActionResult> RestockIngredient(string restaurant, string ingredient, int amount)
    {
        await _kitchen
            .Get(restaurant)
            .Perform(_ => _.RestockIngredient(ingredient, amount))
            .ConfigureAwait(false);
        
        return Ok();
    }
    
    [HttpPost("{restaurant}/chef/{chef}")]
    public async Task<IActionResult> CheckInChef(string restaurant, string chef)
    {
        await _kitchen
            .Get(restaurant)
            .Perform(_ => _.CheckInChef(chef))
            .ConfigureAwait(false);
        
        return Ok();
    }
    
    [HttpDelete("{restaurant}/chef/{chef}")]
    public async Task<IActionResult> CheckOutChef(string restaurant, string chef)
    {
        await _kitchen
            .Get(restaurant)
            .Perform(_ => _.CheckOutChef(chef))
            .ConfigureAwait(false);
        
        return Ok();
    }
}
