// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/projections/csharp/

using System;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle(_ => _ 
        .WithProjections(_ => _
            .Create("0767bc04-bc03-40b8-a0be-5f6c6130f68b")
                .ForReadModel<Chef>()
                .On<DishPrepared>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, projectionContext) =>
                {
                    chef.Name = @event.Chef;
                    if (!chef.Dishes.Contains(@event.Dish)) chef.Dishes.Add(@event.Dish);
                    return chef;
                })
        )
    )
    .Build();
await host.StartAsync();

var client = await host.GetDolittleClient();
var eventStore = client.EventStore.ForTenant(TenantId.Development);
await eventStore.CommitEvent(new DishPrepared("Bean Blaster Taco", "Mr. Taco"), "Dolittle Tacos");
await eventStore.CommitEvent(new DishPrepared("Bean Blaster Taco", "Mrs. Tex Mex"), "Dolittle Tacos");
await eventStore.CommitEvent(new DishPrepared("Avocado Artillery Tortilla", "Mr. Taco"), "Dolittle Tacos");
await eventStore.CommitEvent(new DishPrepared("Chili Canon Wrap", "Mrs. Tex Mex"), "Dolittle Tacos");

await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

var dishes = await client.Projections
    .ForTenant(TenantId.Development)
    .GetAll<DishCounter>().ConfigureAwait(false);

foreach (var (dish, state) in dishes )
{
    Console.WriteLine($"The kitchen has prepared {dish} {state.State.NumberOfTimesPrepared} times");
}

var chef = await client.Projections
    .ForTenant(TenantId.Development)
    .Get<Chef>("Mrs. Tex Mex").ConfigureAwait(false);
Console.WriteLine($"Mrs. Tex Mex has prepared {string.Join(", ", chef.Dishes)}");

await host.WaitForShutdownAsync();

    
