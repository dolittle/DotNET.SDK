// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/projections/csharp/

using System;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;

namespace Kitchen
{
    class Program
    {
        public async static Task Main()
        {
            var client = DolittleClient
                .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventTypes(eventTypes =>
                    eventTypes.Register<DishPrepared>())
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
                })
                .Build();

            var started = client.Start();

            var eventStore = client.EventStore.ForTenant(TenantId.Development);

            await eventStore.Commit(_ =>
                _.CreateEvent(new DishPrepared("Bean Blaster Taco", "Mr. Taco"))
                .FromEventSource("Dolittle Tacos"))
                .ConfigureAwait(false);
            await eventStore.Commit(_ =>
                _.CreateEvent(new DishPrepared("Bean Blaster Taco", "Mrs. Tex Mex"))
                .FromEventSource("Dolittle Tacos"))
                .ConfigureAwait(false);
            await eventStore.Commit(_ =>
                _.CreateEvent(new DishPrepared("Avocado Artillery Tortilla", "Mr. Taco"))
                .FromEventSource("Dolittle Tacos"))
                .ConfigureAwait(false);
            await eventStore.Commit(_ =>
                _.CreateEvent(new DishPrepared("Chili Canon Wrap", "Mrs. Tex Mex"))
                .FromEventSource("Dolittle Tacos"))
                .ConfigureAwait(false);

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

            // Blocks until the EventHandlers are finished, i.e. forever
            await started.ConfigureAwait(false);
        }
    }
}
