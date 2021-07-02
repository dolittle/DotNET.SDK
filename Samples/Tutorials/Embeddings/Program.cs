// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using System;
using System.Threading.Tasks;
using System.Linq;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;

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
                    eventTypes.Register<DishRemoved>();
                    eventTypes.Register<ChefHired>();
                    eventTypes.Register<ChefFired>();
                })
                .WithEmbeddings(builder =>
                {
                    builder.RegisterEmbedding<DishCounter>();
                    // builder
                    //     .CreateEmbedding("999a6aa4-4412-4eaf-a99b-2842cb191e7c")
                    //     .ForReadModel<Chef>()
                    //     .Compare((receivedState, currentState, context) =>
                    //     {
                    //         {
                    //             return new ChefHired(receivedState.Name);
                    //         }
                    //     });
                })
                .Build();
            _ = client.Start();

            await DoStuffWithEmbeddings(client).ConfigureAwait(false);
        }

        static async Task DoStuffWithEmbeddings(Client client)
        {
            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

            var tacoCounter = new DishCounter
            {
                Dish = "Taco",
                NumberOfTimesPrepared = 5
            };
            var burritoCounter = new DishCounter
            {
                Dish = "Burrito",
                NumberOfTimesPrepared = 3
            };

            var tasks = new[] { tacoCounter, burritoCounter }
                .Select(async counter =>
                {
                    Console.WriteLine($"Updating dish: {counter.Dish}");
                    await client.Embeddings
                        .ForTenant(TenantId.Development)
                        .Update(counter.Dish, counter);

                    var counterState = await client.Embeddings
                        .ForTenant(TenantId.Development)
                        .Get<DishCounter>(counter.Dish);
                    Console.WriteLine($"Got dish: {counterState.State}");

                    Console.WriteLine($"Got dish: {counterState.State}");
                    await client.Embeddings
                        .ForTenant(TenantId.Development)
                        .Delete<DishCounter>(counter.Dish);

                    var deletedCounter = await client.Embeddings
                        .ForTenant(TenantId.Development)
                        .Get<DishCounter>(counter.Dish);
                    Console.WriteLine($"Got a deleted, initial dish counter: {deletedCounter.State}");
                }).ToArray();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
