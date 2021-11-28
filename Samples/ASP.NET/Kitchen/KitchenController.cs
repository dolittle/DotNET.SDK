// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Builders;
using Microsoft.AspNetCore.Mvc;

namespace Kitchen;

[Route("/api/kitchen")]
public class KitchenController : ControllerBase
{
    readonly IAggregates _aggregates;

    public KitchenController(IAggregates aggregates)
    {
        _aggregates = aggregates;
    }

    [HttpPost("prepare")]
    public async Task<IActionResult> Prepare([FromBody] PrepareDish cmd)
    {
        await _aggregates
            .Get<Kitchen>(cmd.Kitchen)
            .Perform(_ => _.PrepareDish(cmd.Chef, cmd.Dish))
            .ConfigureAwait(false);

        return Ok();
    }
}
