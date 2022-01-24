// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Projections.Store;
using Microsoft.AspNetCore.Mvc;

namespace Customers;

[Route("/api/customers")]
public class CustomerController : ControllerBase
{
    readonly IProjectionStore _projections;

    public CustomerController(IProjectionStore projections)
    {
        _projections = projections;
    }

    [HttpGet("{customer}")]
    public async Task<string[]> GetDishesEaten([FromRoute]string customer)
    {
        var state = await _projections
            .Get<DishesEaten>(customer, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return state.Dishes;
    }
}
