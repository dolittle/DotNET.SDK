// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Resources;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Restaurant.Menus;

[Route("/api/menus")]
public class Menus : ControllerBase
{
    readonly IAggregateOf<Menu> _menus;
    readonly IResources _resources;

    public Menus(IAggregateOf<Menu> menus, IResources resources)
    {
        _menus = menus;
        _resources = resources;
    }

    [HttpPost("{restaurant}/dishes")]
    public async Task<IActionResult> AddDish(string restaurant, string dish)
    {
        await _menus
            .Get(restaurant)
            .Perform(_ => _.AddDish(dish))
            .ConfigureAwait(false);

        return Ok();
    }
    
    [HttpDelete("{restaurant}/dishes")]
    public async Task<IActionResult> RemoveDish(string restaurant, string dish)
    {
        await _menus
            .Get(restaurant)
            .Perform(_ => _.RemoveDish(dish))
            .ConfigureAwait(false);

        return Ok();
    }

    [HttpGet("{restaurant}")]
    public async Task<ActionResult<IEnumerable<RestaurantMenu.Menu.Dish>>> GetMenu(string restaurant)
    {
        var database = await _resources.MongoDB.GetDatabase().ConfigureAwait(false);
        var menus = database.GetCollection<RestaurantMenu.Menu>("menus");
        var menu = await menus.Find(Builders<RestaurantMenu.Menu>.Filter.Eq(_ => _.Restaurant, restaurant))
            .SingleAsync();

        return Ok(menu.Dishes);
    }
}