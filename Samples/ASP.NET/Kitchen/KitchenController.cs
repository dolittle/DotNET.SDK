// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Projections.Store;
using Microsoft.AspNetCore.Mvc;

namespace Kitchen;

[Route("/api/kitchen")]
public class KitchenController : ControllerBase
{
    readonly IAggregateOf<Kitchen> _kitchen;
    private readonly IProjectionOf<Pantry> _pantryClient;

    public KitchenController(IAggregateOf<Kitchen> kitchen, IProjectionOf<Pantry> pantryClient)
    {
        _kitchen = kitchen;
        _pantryClient = pantryClient;
    }

    [HttpGet("{kitchen}/pantry")]
    public async Task<IActionResult> Get([FromRoute] string kitchen)
    {
        var pantry = await _pantryClient
            .Get(kitchen)
            .ConfigureAwait(false);

        return pantry is null ? NotFound() : Ok(pantry);
    }

    [HttpGet("{kitchen}/pantry-updates")]
    public async Task<IActionResult> GetSomeUpdates([FromRoute] string kitchen, CancellationToken cancellationToken)
    {
        // using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        using var updates = _pantryClient
            .Subscribe<Pantry>(kitchen, cancellationToken);

        return Ok(updates.Channel.ReadAllAsync(cancellationToken).ToBlockingEnumerable().Take(2).ToList());
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