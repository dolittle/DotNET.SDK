// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Projections;
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
                    eventTypes.Register<DishAdded>();
                    eventTypes.Register<DishPrepared>();
                    eventTypes.Register<DishRemoved>();
                    eventTypes.Register<ChefHired>();
                    eventTypes.Register<ChefFired>();
                })
                .WithEmbeddings(builder =>
                {
                    builder.RegisterEmbedding<DishCounter>();
                    builder
                        .CreateEmbedding("999a6aa4-4412-4eaf-a99b-2842cb191e7c")
                        .ForReadModel<Chef>()
                        .ResolveUpdateToEvents((updatedState, currentState, context) =>
                        {
                            return new ChefHired(updatedState.Name);
                        })
                        .ResolveDeletionToEvents((currentState, context) => new ChefFired(currentState.Name))
                        .On<ChefHired>((updatedState, @event, projectionContext) =>
                        {
                            updatedState.Name = @event.Chef;
                            return updatedState;
                        })
                        .On<ChefFired>((currentState, @event, projectionContext) => ProjectionResult<Chef>.Delete);
                })
                .Build();
            _ = client.Start();

            await DoStuffWithEmbeddings(client).ConfigureAwait(false);
        }

        static async Task DoStuffWithEmbeddings(Client client)
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

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

            Console.WriteLine("Hiring some chefs");
            var mrTaco = new Chef
            {
                Name = "Mr. Taco"
            };
            var mrsTexMex = new Chef
            {
                Name = "Mrs. TexMex"
            };

            await client.Embeddings
                .ForTenant(TenantId.Development)
                .Update(mrTaco.Name, mrTaco);
            await client.Embeddings
                .ForTenant(TenantId.Development)
                .Update(mrsTexMex.Name, mrsTexMex);
            var mrTacoState = await client.Embeddings
                .ForTenant(TenantId.Development)
                .Get<Chef>(mrTaco.Name);
            Console.WriteLine($"Got Mr. Taco: {mrTacoState.State.Name}");

            var mrsTexMexState = await client.Embeddings
                .ForTenant(TenantId.Development)
                .Get<Chef>(mrsTexMex.Name);
            Console.WriteLine($"Got Mrs. TexMex: {mrsTexMexState.State.Name}");

            await client.Embeddings
                .ForTenant(TenantId.Development)
                .Delete<Chef>(mrTaco.Name);

            var deletedState = await client.Embeddings
                .ForTenant(TenantId.Development)
                .Get<Chef>(mrTaco.Name);
            Console.WriteLine($"Got deleted Mr. Taco: {deletedState.State.Name}");
        }
    }
}
