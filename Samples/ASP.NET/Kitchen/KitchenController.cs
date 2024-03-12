// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Microsoft.AspNetCore.Mvc;

namespace Kitchen;

[Route("/api/kitchen")]
public class KitchenController : ControllerBase
{
    readonly IAggregateOf<Kitchen> _kitchen;

    public KitchenController(IAggregateOf<Kitchen> kitchen)
    {
        _kitchen = kitchen;
    }

    [HttpPost("prepare")]
    public async Task<IActionResult> Prepare([FromBody] PrepareDish cmd)
    {
        try
        {
            var remaining = await _kitchen
                .Get(cmd.Kitchen)
                .Perform(_ => _.PrepareDish(cmd.Chef, cmd.Dish))
                .ConfigureAwait(false);
            return Ok(remaining);
        }
        catch (AggregateRootOperationFailed e) when (e.InnerException is OutOfIngredients)
        {
            return StatusCode(400, e.InnerException.Message);
        }
    }
}