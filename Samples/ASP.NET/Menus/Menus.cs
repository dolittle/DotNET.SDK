// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Menus;

[Route("/api/menus")]
public class Menus : ControllerBase
{
    readonly IAggregateOf<Menu> _menus;

    public Menus(IAggregateOf<Menu> menus)
    {
        _menus = menus;
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
}