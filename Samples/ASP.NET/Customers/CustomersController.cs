// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK.Projections.Store;
using Microsoft.AspNetCore.Mvc;

namespace Customers;

[Route("/api/customers")]
public class CustomerController : ControllerBase
{
    readonly IProjectionOf<DishesEaten> _dishesEaten;

    public CustomerController(IProjectionOf<DishesEaten> dishesEaten)
    {
        _dishesEaten = dishesEaten;
    }

    [HttpGet("{customer}")]
    public async Task<string[]> GetDishesEaten([FromRoute] string customer)
    {
        var state = await _dishesEaten
            .Get(customer, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return state?.Dishes ?? [];
    }
    
    [HttpGet("All")]
    public async Task<List<DishesEaten>> GetDishesEaten()
    {
        var states = _dishesEaten
            .Query()
            .ToList();

        return states;
    }
    
}