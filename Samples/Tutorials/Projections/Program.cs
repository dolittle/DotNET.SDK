// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/projections/csharp/

using System;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle(clientBuilder => clientBuilder
        .WithEventTypes(eventTypes => eventTypes.Register<DishPrepared>())
        .WithProjections(builder =>
        {
            builder.RegisterProjection<DishCounter>();

            builder.CreateProjection("0767bc04-bc03-40b8-a0be-5f6c6130f68b")
                .ForReadModel<Chef>()
                .On<DishPrepared>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, projectionContext) =>
                {
                    chef.Name = @event.Chef;
                    if (!chef.Dishes.Contains(@event.Dish)) chef.Dishes.Add(@event.Dish);
                    return chef;
                });
        }))
    .Build();
await host.StartAsync();

var client = await host.GetDolittleClient();
var eventStore = client.EventStore.ForTenant(TenantId.Development);
await eventStore.Commit(_ =>
{
    AddPreparedDishFromDolittleTacos(_, "Bean Blaster Taco", "Mr. Taco");
    AddPreparedDishFromDolittleTacos(_, "Bean Blaster Taco", "Mrs. Tex Mex");
    AddPreparedDishFromDolittleTacos(_, "Avocado Artillery Tortilla", "Mr. Taco");
    AddPreparedDishFromDolittleTacos(_, "Chili Canon Wrap", "Mrs. Tex Mex");
});
await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

var dishes = await client.Projections
    .ForTenant(TenantId.Development)
    .GetAll<DishCounter>().ConfigureAwait(false);

foreach (var (dish, state) in dishes)
{
    Console.WriteLine($"The kitchen has prepared {dish} {state.State.NumberOfTimesPrepared} times");
}

var chef = await client.Projections
    .ForTenant(TenantId.Development)
    .Get<Chef>("Mrs. Tex Mex").ConfigureAwait(false);
Console.WriteLine($"{chef.Key} has prepared {string.Join(", ", chef.State.Dishes)}");

await host.WaitForShutdownAsync();

static void AddPreparedDishFromDolittleTacos(UncommittedEventsBuilder builder, string dish, string chef)
    => builder.CreateEvent(new DishPrepared(dish, chef)).FromEventSource("Dolittle Tacos");
    