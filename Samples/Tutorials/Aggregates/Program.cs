﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;


var host = Host.CreateDefaultBuilder()
    .UseDolittle(clientBuilder => clientBuilder
        .WithEventTypes(eventTypes => eventTypes.Register<DishPrepared>())
        .WithEventHandlers(eventHandlers => eventHandlers.RegisterEventHandler<DishHandler>())
        .WithAggregateRoots(aggregateRoots => aggregateRoots.Register<Kitchen>()))
    .Build();

await host.StartAsync();

var client = await host.GetDolittleClient();
await client.EventStore
    .ForTenant(TenantId.Development)
    .Commit(eventsBuilder => eventsBuilder
        .CreateEvent(new DishPrepared("Bean Blaster Taco", "Mr. Taco"))
        .FromEventSource("Dolittle Tacos"));

await client.Aggregates
    .ForTenant(TenantId.Development)
    .Get<Kitchen>("Dolittle Tacos")
    .Perform(kitchen => kitchen.PrepareDish("Bean Blaster Taco", "Mr. Taco"));

await host.WaitForShutdownAsync();
