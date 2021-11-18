// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Kitchen
{
    [Route("/api/kitchen")]
    public class KitchenController : ControllerBase
    {
        DolittleClient _client;

        public KitchenController(DolittleClient client)
        {
            _client = client;
        }

        [HttpPost("prepare")]
        public async Task<IActionResult> Prepare([FromBody] PrepareDish prepareDish)
        {
            var preparedDish = new DishPrepared(prepareDish.Dish, prepareDish.Chef);

            await _client.EventStore
                .ForTenant(TenantId.Development)
                .Commit(eventsBuilder =>
                    eventsBuilder
                        .CreateEvent(preparedDish)
                        .FromEventSource("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9"));

            return Ok();
        }
    }

}
