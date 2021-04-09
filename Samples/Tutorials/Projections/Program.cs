// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

using System;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Tenancy;
using Newtonsoft.Json.Linq;

namespace Kitchen
{
    class Program
    {
        public static async Task Main()
        {
            var client = Client
                .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventTypes(eventTypes =>
                {
                    eventTypes.Register<DishPrepared>();
                    eventTypes.Register<ChefFired>();
                })
                .WithProjections(projections =>
                {
                    projections.CreateProjection("4a4c5b13-d4dd-4665-a9df-27b8e9b2054c")
                        .ForReadModel<Chef>()
                        .On<DishPrepared>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, ctx) =>
                        {
                            chef.Name = @event.Chef;
                            chef.Dishes.Add(@event.Dish);
                            return chef;
                        })
                        .On<ChefFired>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, ctx) =>
                        {
                            return ProjectionResult<Chef>.Delete;
                        });
                    projections.RegisterProjection<Menu>();
                })
                // .WithProjections(projections =>
                // {
                //     projections.CreateProjection("4a4c5b13-d4dd-4665-a9df-27b8e9b2054c")
                //         .ForReadModel<Chef>()
                //         .On<DishPrepared>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, ctx) =>
                //         {
                //             chef.Name = @event.Chef;
                //             chef.Dishes.Add(@event.Dish);
                //             return chef;
                //         })
                //         .On<ChefFired>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, ctx) =>
                //         {
                //             return ProjectionResult<Chef>.Delete;
                //         });
                //     projections.RegisterProjection<Menu>();
                // })
                .Build();

            var preparedTaco = new DishPrepared("Bean Blaster Taco", "Mr. Taco");
            var avocadoArtillery = new DishPrepared("Avocado Artillery Tortilla", "Mr. Taco");
            var chiliCannon = new DishPrepared("Chili Cannon Wrap", "Ms. TexMex");
            // var mrTacoFired = new ChefFired("Mr. Taco");

            await client.EventStore
                .ForTenant(TenantId.Development)
                .Commit(eventsBuilder =>
                {
                    eventsBuilder
                        .CreateEvent(preparedTaco)
                        .FromEventSource("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9");
                    eventsBuilder
                        .CreateEvent(avocadoArtillery)
                        .FromEventSource("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9");
                    eventsBuilder
                        .CreateEvent(chiliCannon)
                        .FromEventSource("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9");
                    // eventsBuilder
                    //     .CreateEvent(mrTacoFired)
                    //     .FromEventSource("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9");
                }).ConfigureAwait(false);

            var started = client.Start();

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(true);

            Console.WriteLine("Getting menu");
            var menu = await client.Projections.ForTenant(TenantId.Development).Get<Menu>("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9").ConfigureAwait(false);
            Console.WriteLine($"Menu consists of: {string.Join(", ", menu.State.Dishes)}");

            Console.WriteLine("Getting untyped menu");
            var untypedMenu = await client.Projections.ForTenant(TenantId.Development).Get("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9", "0405b93f-1461-472c-bdc2-f89e0afd4dfe").ConfigureAwait(false);
            Console.WriteLine($"Menu is ({untypedMenu.State.GetType()}) {untypedMenu.State}");

            Console.WriteLine("Getting all chefs");
            foreach (var (key, state) in await client.Projections.ForTenant(TenantId.Development).GetAll<Chef>().ConfigureAwait(false))
            {
                var chef = state.State;
                Console.WriteLine($"Chef name: {chef.Name} and prepared dishes: {string.Join(",", chef.Dishes)}");
            }

            Console.WriteLine("Getting all chefs as menus");
            foreach (var (key, state) in await client.Projections.ForTenant(TenantId.Development).GetAll<Menu>("4a4c5b13-d4dd-4665-a9df-27b8e9b2054c").ConfigureAwait(false))
            {
                Console.WriteLine($"Menu {key} consists of: {string.Join(", ", state.State.Dishes)}");
            }

            await started.ConfigureAwait(false);
        }
    }
}
