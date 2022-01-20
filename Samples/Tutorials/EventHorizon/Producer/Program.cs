﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle(_ => _
        .WithFilters(_ => _
            .CreatePublic("2c087657-b318-40b1-ae92-a400de44e507")
            .Handle((@event, eventContext) =>
            {
                Console.WriteLine($"Filtering event {@event} to public streams");
                return Task.FromResult(new PartitionedFilterResult(true, eventContext.EventSourceId.Value));
            })))
    .Build();

await host.StartAsync();

var client = await host.GetDolittleClient();
var preparedTaco = new DishPrepared("Bean Blaster Taco", "Mr. Taco");
await client.EventStore
    .ForTenant(TenantId.Development)
    .CommitPublicEvent(preparedTaco, "Dolittle Tacos");

await host.WaitForShutdownAsync();
