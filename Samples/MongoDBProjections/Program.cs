// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

var host = Host.CreateDefaultBuilder()
    .UseDolittle()
    .Build();
await host.StartAsync();

var client = await host.GetDolittleClient();
var eventStore = client.EventStore.ForTenant(TenantId.Development);
await eventStore.CommitEvent(new DishPrepared("Bean Blaster Taco", "Mr. Taco"), "Dolittle Tacos");
await eventStore.CommitEvent(new DishPrepared("Bean Blaster Taco", "Mrs. Tex Mex"), "Dolittle Tacos");
await eventStore.CommitEvent(new DishPrepared("Avocado Artillery Tortilla", "Mr. Taco"), "Dolittle Tacos");
await eventStore.CommitEvent(new DishPrepared("Chili Canon Wrap", "Mrs. Tex Mex"), "Dolittle Tacos");

await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

var dishCounterCollection = client.Services.ForTenant(TenantId.Development).GetRequiredService<IMongoCollection<DishCounter>>();

foreach (var dish in await dishCounterCollection.Find(FilterDefinition<DishCounter>.Empty).ToListAsync().ConfigureAwait(false))
{
    Console.WriteLine($"The kitchen has prepared {dish.Name} {dish.NumberOfTimesPrepared} times. The last time was {dish.LastPrepared.ToLocalTime()}");
}

var dishesPreparedToday = await dishCounterCollection.Find(_ => _.LastPrepared >= DateTime.Today && _.LastPrepared < DateTime.Today.AddDays(1)).ToListAsync();

foreach (var dish in dishesPreparedToday)
{
    Console.WriteLine($"The kitchen has prepared {dish.Name} today");
}

await host.WaitForShutdownAsync();

